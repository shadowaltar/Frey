using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Statistics;
using Trading.Backtest.Data;
using Trading.Backtest.Reporting;
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

        public DateTime TestStart { get; private set; }
        public DateTime TestEnd { get; private set; }
        public DateTime EndOfData { get; private set; }
        public DateTime CurrentDate { get; private set; }

        public double AnnualizedReturn { get; private set; }
        public double AnnualizedVolatility { get; private set; }
        public double AnnualizedSharpeRatio { get; private set; }
        public HashSet<int> Holidays { get; private set; }

        private Dictionary<long, Dictionary<DateTime, double>> positionPriceHistory = new Dictionary<long, Dictionary<DateTime, double>>();
        public Dictionary<long, Dictionary<DateTime, double>> PositionPriceHistory { get { return positionPriceHistory; } }

        private long spySecid;

        public Core(double money)
        {
            PortfolioEquity = money;
            InitialPortfolioEquity = money;
            Positions = new HashSet<Position>();
            PositionReportItems = new List<PositionReportEntry>();
            PortfolioStatuses = new List<PortfolioStatus>();
            Trades = new List<Trade>();
            Holidays = new HashSet<int>();

            AnnualizedReturn = double.NaN;
            AnnualizedVolatility = double.NaN;
            AnnualizedSharpeRatio = double.NaN;
        }

        public void Initialize(DateTime testStart, DateTime testEnd, DateTime endOfData)
        {
            TestStart = testStart;
            TestEnd = testEnd;
            EndOfData = endOfData;
            CurrentDate = TestStart.Next(DayOfWeek.Tuesday).Next(DayOfWeek.Tuesday);
            spySecid = DataCache.SecurityCache[DataCache.SecurityCodeMap["^GSPC"]].Id;

            var firstPriceDate = TestStart;
            var prices = SkipNoPriceDates(ref firstPriceDate);
            var spyPrice = prices[spySecid].AdjClose;
            PortfolioStatuses.Add(new PortfolioStatus(firstPriceDate, PortfolioEquity, spyPrice));
            positionPriceHistory = new Dictionary<long, Dictionary<DateTime, double>>();
        }

        public void Run()
        {
            do
            {
                if (Positions.Count != 0)
                    ExitPositions();
                if (!NextTuesday())
                    return;

                if (!EnterPositions())
                    break;
            } while (Next());
            Finish();
            CalculateStatistics();
        }

        public bool EnterPositions()
        {
            return EnterPositions(CurrentDate);
        }

        public bool EnterPositions(DateTime tradeDate)
        {
            var start = tradeDate.Previous(DayOfWeek.Tuesday);

            var dateOnePrices = SkipNoPriceDates(ref start);
            if (dateOnePrices == null)
                return false;
            var excludedDateOnePrices = dateOnePrices.Where(p => !SatisfyBuySell(p.Value))
                .Select(p => p.Key).ToHashSet();


            var dateTwoPrices = SkipNoPriceDates(ref tradeDate);
            if (dateTwoPrices == null)
                return false;

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

            var bottom10 = returns.OrderBy(pair => pair.Value).Take(20);

            foreach (var pair in bottom10)
            {
                var sid = pair.Key;
                var newPosition = new Position();
                newPosition.Price = dateTwoPrices[sid].AdjClose;
                newPosition.Time = tradeDate;
                var q = Math.Floor(PortfolioEquity / 20 / newPosition.Price);
                if (q < 1)
                    continue;
                newPosition.Quantity = q;
                newPosition.Security = DataCache.SecurityCache[sid];
                Positions.Add(newPosition);
                PortfolioEquity -= newPosition.Value;
            }

            // mark down the price history for these positions
            foreach (var p in Positions)
            {
                var t = p.Time;
                var sid = p.Security.Id;
                Dictionary<long, Price> pricesOfDay;
                Dictionary<DateTime, double> pricesOfSecurity;

                t = t.AddDays(-7);
                for (int i = 0; i < 14; i++) // for prev&next week prices
                {
                    if (DataCache.PriceCache.TryGetValue(t, out pricesOfDay) && pricesOfDay.Count != 0)
                    {
                        if (!positionPriceHistory.TryGetValue(sid, out pricesOfSecurity))
                        {
                            positionPriceHistory[sid] = new Dictionary<DateTime, double>();
                        }
                        if (pricesOfDay.ContainsKey(sid))
                        {
                            positionPriceHistory[sid][t] = pricesOfDay[sid].AdjClose;
                        }
                    }
                    t = t.AddDays(1);
                }
            }

            // report
            PositionReportItems.AddRange(Positions.Select(p => new PositionReportEntry
            {
                Price = p.Price,
                Quantity = (int)p.Quantity,
                Time = p.Time.ToDateInt(),
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
            var close = time.Next(DayOfWeek.Tuesday);
            if (close > TestEnd || close > EndOfData)
            {
                close = EndOfData > TestEnd ? TestEnd : EndOfData;
                if (Positions.Count == 0 || Positions.Any(p => p.Time == close))
                {
                    Positions.Clear();
                    return true;
                }
            }

            foreach (var position in Positions)
            {
                if (!DataCache.PriceCache.ContainsKey(close))
                    throw new InvalidOperationException();
                if (position.Time >= close)
                    throw new InvalidOperationException();

                var prices = SkipNoPriceDates(ref close);
                if (prices == null)
                    return false;

                if (!prices.ContainsKey(position.Security.Id))
                    throw new InvalidOperationException();
                var price = prices[position.Security.Id].AdjClose;

                var trade = new Trade
                {
                    Security = position.Security,
                    EnterTime = position.Time,
                    ExitTime = close,
                    EnterPrice = position.Price,
                    ExitPrice = price,
                    Quantity = position.Quantity,
                    ExitType = ExitType.Close,
                };
                Trades.Add(trade);
                PortfolioEquity += trade.Value;
            }
            Positions.Clear();
            var spyPrice = DataCache.PriceCache[close][spySecid].AdjClose;
            PortfolioStatuses.Add(new PortfolioStatus(close, PortfolioEquity, spyPrice));
            return true;
        }

        private Dictionary<long, Price> SkipNoPriceDates(ref DateTime close)
        {
            var prices = DataCache.PriceCache[close];
            var shiftCounter = 0;
            while (prices.Count == 0)
            {
                close = close.AddDays(1);
                shiftCounter++;
                if (DataCache.PriceCache.ContainsKey(close))
                {
                    prices = DataCache.PriceCache[close];
                    break;
                }
                if (shiftCounter > 7)
                {
                    return null;
                }
            }
            return prices;
        }

        private bool SatisfyBuySell(Price p)
        {
            return p.Volume > 200000 && p.AdjClose > 10 && p.Close > 10;
        }

        public bool Next()
        {
            CurrentDate = CurrentDate.AddDays(1);
            return CurrentDate <= TestEnd || CurrentDate <= EndOfData;
        }

        public bool NextTuesday()
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