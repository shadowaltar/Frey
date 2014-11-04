using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CsvHelper;
using MathNet.Numerics.Statistics;
using Trading.Backtest.Data;
using Trading.Backtest.Reporting;
using Trading.Common;
using Trading.Common.Entities;
using Trading.Common.Utils;

namespace Trading.Backtest.ViewModels
{
    public class Core
    {
        public HashSet<Position> Positions { get; private set; }
        public List<PositionReportEntry> PositionReportItems { get; private set; }
        public List<PortfolioStatus> PortfolioStatuses { get; private set; }
        public List<Trade> Trades { get; set; }
        public double PortfolioEquity { get; set; }
        public double InitialPortfolioEquity { get; set; }
        public double RemainingCash { get; set; }

        public int LongPositionCount { get; private set; }
        public int ShortPositionCount { get; private set; }

        public DateTime TestStart { get; private set; }
        public DateTime TestEnd { get; private set; }
        public DateTime EndOfData { get; private set; }
        public DateTime CurrentDate { get; private set; }

        public double AnnualizedReturn { get; private set; }
        public double AnnualizedVolatility { get; private set; }
        public double AnnualizedSharpeRatio { get; private set; }

        public Core(double money)
        {
            PortfolioEquity = money;
            InitialPortfolioEquity = money;
            Positions = new HashSet<Position>();
            PositionReportItems = new List<PositionReportEntry>();
            PortfolioStatuses = new List<PortfolioStatus>();
            Trades = new List<Trade>();

            AnnualizedReturn = double.NaN;
            AnnualizedVolatility = double.NaN;
            AnnualizedSharpeRatio = double.NaN;
        }

        public void SetDays(DateTime testStart, DateTime testEnd, DateTime endOfData)
        {
            TestStart = testStart;
            TestEnd = testEnd;
            EndOfData = endOfData;
            CurrentDate = TestStart.Next(DayOfWeek.Tuesday).Next(DayOfWeek.Tuesday);
            PortfolioStatuses.Add(new PortfolioStatus(TestStart, PortfolioEquity));
        }

        public bool EnterPositions()
        {
            return EnterPositions(CurrentDate);
        }

        public bool EnterPositions(DateTime start)
        {
            var prices = DataCache.PriceCache;

            start = start.Previous(DayOfWeek.Tuesday);

            var dateOnePrices = prices[start];
            var shiftCounter = 0;
            while (dateOnePrices.Count == 0)
            {
                //     Console.WriteLine("No security has price on #1 " + start.IsoFormat() + " so use next date.");
                start = start.AddDays(-1);
                shiftCounter++;
                if (prices.ContainsKey(start))
                    dateOnePrices = prices[start];
                if (shiftCounter > 7)
                    return false;
            }
            var excludedDateOnePrices = dateOnePrices.Where(p => !SatisfyBuySell(p.Value))
                .Select(p => p.Key).ToHashSet();

            var tradeDate = start.Next(DayOfWeek.Tuesday);
            var dateTwoPrices = prices[tradeDate];
            shiftCounter = 0;
            while (dateTwoPrices.Count == 0)
            {
                //      Console.WriteLine("No security has price on #2 " + tradeDate.IsoFormat() + " so use next date.");
                tradeDate = tradeDate.AddDays(1);
                shiftCounter++;
                if (prices.ContainsKey(tradeDate))
                    dateTwoPrices = prices[tradeDate];
                if (shiftCounter > 7)
                    return false;
            }

            if (tradeDate >= EndOfData || tradeDate >= TestEnd)
                return false;

            var bigVolumeDateTwo = dateTwoPrices.Where(p => SatisfyBuySell(p.Value));

            var returns = new Dictionary<long, double>(); // secid-return
            foreach (var pair in bigVolumeDateTwo)
            {
                var sid = pair.Key;
                if (excludedDateOnePrices.Contains(sid)) // filter out those volume too small on day 1
                    continue;
                if (!dateOnePrices.ContainsKey(sid)) // filter out those not exist at all on day 1
                    continue;

                returns[sid] = Math.Log(dateTwoPrices[sid].AdjClose / dateOnePrices[sid].AdjClose);
            }

            var top10 = returns.OrderByDescending(pair => pair.Value).Take(10).ToList();
            var bottom10 = returns.OrderBy(pair => pair.Value).Take(10);

            foreach (var pair in bottom10)
            {
                var sid = pair.Key;
                var newPosition = new Position();
                newPosition.Price = dateTwoPrices[sid].AdjClose;
                newPosition.Time = tradeDate;
                var q = Math.Floor(PortfolioEquity / 10 / newPosition.Price);
                if (q < 1)
                    continue;
                newPosition.Quantity = q;
                newPosition.Security = DataCache.SecurityCache[sid];
                Positions.Add(newPosition);
                PortfolioEquity -= newPosition.Value;
            }

            // report
            PositionReportItems.AddRange(Positions.Select(p => new PositionReportEntry
            {
                Price = p.Price.ToString("N2"),
                Quantity = p.Quantity.ToString("N0"),
                Time = p.Time.IsoFormat(),
                SecurityCode = p.Security.Code,
                SecurityName = p.Security.Name,
            }));
            return true;
        }

        public bool ExitPositions()
        {
            if (CurrentDate > TestEnd || CurrentDate > EndOfData)
            {
                CurrentDate = EndOfData > TestEnd ? TestEnd : EndOfData;
            }
            return ExitPositions(CurrentDate);
        }

        public bool Finish()
        {
            return ExitPositions(EndOfData);
        }

        public bool ExitPositions(DateTime time)
        {
            foreach (var position in Positions)
            {
                if (!DataCache.PriceCache.ContainsKey(time))
                    throw new InvalidOperationException();
                if (position.Time >= time)
                    throw new InvalidOperationException();

                var prices = DataCache.PriceCache[time];
                var shiftCounter = 0;
                while (prices.Count == 0)
                {
                    //   Console.WriteLine("No security has price on # " + time.IsoFormat() + " so use next date.");
                    time = time.AddDays(1);
                    shiftCounter++;
                    if (DataCache.PriceCache.ContainsKey(time))
                        prices = DataCache.PriceCache[time];
                    if (shiftCounter > 7)
                        return false;
                }

                if (!prices.ContainsKey(position.Security.Id))
                    throw new InvalidOperationException();
                var price = prices[position.Security.Id].AdjClose;

                var trade = new Trade
                {
                    Security = position.Security,
                    EnterTime = position.Time,
                    ExitTime = time,
                    EnterPrice = position.Price,
                    ExitPrice = price,
                    Quantity = position.Quantity,
                    ExitType = ExitType.Close,
                };
                Trades.Add(trade);
                PortfolioEquity += trade.Value;
            }
            Positions.Clear();
            PortfolioStatuses.Add(new PortfolioStatus(time, PortfolioEquity));
            Console.WriteLine(time + "  " + PortfolioEquity);
            return true;
        }

        private bool SatisfyBuySell(Price p)
        {
            return p.Volume > 200000 && p.AdjClose > 10 && p.Close > 10;
        }

        public bool Next()
        {
            CurrentDate = CurrentDate.Next(DayOfWeek.Tuesday);
            return CurrentDate <= TestEnd || CurrentDate <= EndOfData;
        }

        public void CalculateStatistics()
        {
            var days = (TestEnd - TestStart).Days;
            var years = days / 365.25;
            var weekToYearFactor = Math.Sqrt(252d / 5d);
            var annualReturn = Math.Pow(PortfolioEquity / InitialPortfolioEquity, 1.0 / years) - 1;
            var annualReturnStd = 0d;
            var sharpe = double.NaN;
            var ptfReturns = new List<double>();
            if (PortfolioStatuses.Count != 1)
            {
                var lastEquity = InitialPortfolioEquity;
                for (int i = 1; i < PortfolioStatuses.Count; i++)
                {
                    var equity = PortfolioStatuses[i].Equity;
                    ptfReturns.Add((equity - lastEquity) / lastEquity);
                    lastEquity = equity;
                }
                annualReturnStd = ptfReturns.StandardDeviation();
                annualReturnStd *= weekToYearFactor;
                sharpe = annualReturn / annualReturnStd;
            }

            AnnualizedReturn = annualReturn;
            AnnualizedVolatility = annualReturnStd;
            AnnualizedSharpeRatio = sharpe;
        }
    }
}