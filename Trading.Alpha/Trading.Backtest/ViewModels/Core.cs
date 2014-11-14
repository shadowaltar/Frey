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

        private List<long> nonTradeables = new List<long>();

        public DateTime TestStart { get; private set; }
        public DateTime TestEnd { get; private set; }
        public DateTime EndOfData { get; private set; }
        public DateTime CurrentDate { get; private set; }

        public double AnnualizedReturn { get; private set; }
        public double AnnualizedVolatility { get; private set; }
        public double AnnualizedSharpeRatio { get; private set; }
        public HashSet<int> Holidays { get; private set; }

        public DayOfWeek WeeklyTradeDay { get { return DayOfWeek.Thursday; } }
        public int OneSidePositionCount { get { return 30; } }
        public int MovingAverageDays { get { return 10; } }
        public int SnpMovingAverageDays { get { return 200; } }

        private long snpSid;
        private BacktestDataAccess access;

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

        public void Initialize(DateTime testStart, DateTime testEnd, DateTime endOfData, BacktestDataAccess access)
        {
            TestStart = testStart;
            TestEnd = testEnd;
            EndOfData = endOfData;
            CurrentDate = TestStart.Next(WeeklyTradeDay).Next(WeeklyTradeDay);
            snpSid = DataCache.SecurityCache[DataCache.SecurityCodeMap[DataCache.CommonBenchmarkCode]].Id;

            this.access = access;
            var firstPriceDate = TestStart;
            var prices = SkipNoPriceDates(ref firstPriceDate);
            var snpPrice = prices[snpSid].AdjClose;
            PortfolioStatuses.Add(new PortfolioStatus(firstPriceDate, PortfolioEquity, snpPrice));

            // we cannot trade indexes.
            nonTradeables.AddRange(DataCache.SecurityCodeMap.Where(p => p.Key.Contains("^")).Select(p => p.Value));
        }

        public void Run()
        {
            do
            {
                if (Positions.Count != 0)
                    ExitPositions();
                if (!NextWeeklyTradeDay())
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
            var start = tradeDate.Previous(WeeklyTradeDay);

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

            var maDays = MovingAverageDays; // a 20-MA
            var maPriceDateLowerBound = tradeDate.AddDays(-MovingAverageDays * 2);
            var maPrices = DataCache.PriceCache.Where(p => p.Key <= tradeDate && p.Key >= maPriceDateLowerBound && p.Value.Count > 0)
                .OrderBy(p => p.Key).Take(maDays);
            var pricesForMa = new Dictionary<long, List<double>>();
            var mas = new Dictionary<long, double>();
            foreach (var p in maPrices)
            {
                foreach (var p2 in p.Value)
                {
                    if (!pricesForMa.ContainsKey(p2.Key))
                    {
                        pricesForMa[p2.Key] = new List<double>();
                    }
                    pricesForMa[p2.Key].Add(p2.Value.AdjClose);
                }
            }
            foreach (var p in pricesForMa)
            {
                mas[p.Key] = p.Value.Average();
            }

            var maPrices2 = DataCache.PriceCache.Where(p => p.Key <= tradeDate && p.Value.Count > 0)
                .OrderBy(p => p.Key).Take(SnpMovingAverageDays);
            var snpMaPrices = new List<double>();
            foreach (var map in maPrices2)
            {
                snpMaPrices.Add(map.Value[snpSid].AdjClose);
            }
            var snpPrice = dateTwoPrices[snpSid].AdjClose;
            var snpMaPrice = snpMaPrices.Count > 80 ? snpMaPrices.Average() : snpPrice;

            var aboveMa = new HashSet<long>();
            var belowMa = new HashSet<long>();
            foreach (var p in mas)
            {
                if (dateTwoPrices.ContainsKey(p.Key) && dateTwoPrices[p.Key].AdjClose < p.Value)
                    aboveMa.Add(p.Key);
                else
                    belowMa.Add(p.Key);
            }

            var positionCount = OneSidePositionCount;
            var allLosers = returns.Where(rp => aboveMa.Contains(rp.Key) && !nonTradeables.Contains(rp.Key) && rp.Value < 0).OrderBy(pair => pair.Value);
            var bottoms = allLosers.Take(positionCount);

            var allWinners = returns.Where(rp => belowMa.Contains(rp.Key) && !nonTradeables.Contains(rp.Key) && rp.Value > 0).OrderByDescending(pair => pair.Value);
            var tops = allWinners.Take(positionCount);

            var cash = PortfolioEquity;
            // long bottoms
            foreach (var pair in bottoms)
            {
                var sid = pair.Key;
                var newPosition = new Position
                {
                    Parameters = new[] { returns[sid], mas[sid] },
                    Price = dateTwoPrices[sid].AdjClose,
                    Time = tradeDate
                };

                var q = Math.Floor(cash / positionCount / newPosition.Price);
                if (q < 1)
                    continue;
                newPosition.Quantity = q;
                newPosition.Security = DataCache.SecurityCache[sid];
                Positions.Add(newPosition);
                PortfolioEquity -= newPosition.Value;
            }
            //if (snpMaPrice < snpPrice)
            //{
            //    return true;
            //}
            // short tops
            foreach (var pair in tops)
            {
                var sid = pair.Key;
                var newPosition = new Position
                {
                    Parameters = new[] { returns[sid], double.NaN },
                    Price = dateTwoPrices[sid].AdjClose,
                    Time = tradeDate
                };

                var q = Math.Floor(cash / positionCount / newPosition.Price);
                if (q < 1)
                    continue;
                newPosition.Quantity = -q;
                newPosition.Security = DataCache.SecurityCache[sid];
                Positions.Add(newPosition);
                PortfolioEquity -= newPosition.Value;
            }

            Console.WriteLine("Remaining cash [{0}] {1}", tradeDate.ToDateInt(), PortfolioEquity);

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
            // stop loss logic
            DateTime checkStopLossDay = time;
            if (time.IsWeekend()) // if nonweekend, already added 1 day
            {
                checkStopLossDay = time.Next(DayOfWeek.Monday);
            }

            var checkStopLossPrices = SkipNoPriceDates(ref checkStopLossDay);
            var stopLossPositions = new List<Position>();
            foreach (var position in Positions)
            {
                if (!checkStopLossPrices.ContainsKey(position.Security.Id))
                    continue;

                var priceToCheck = checkStopLossPrices[position.Security.Id].AdjClose;
                var rate = (priceToCheck - position.Price) / position.Price * Math.Sign(position.Quantity);
                if (rate < -0.02)
                {
                    // stop loss
                    stopLossPositions.Add(position);
                    var trade = new Trade
                    {
                        Security = position.Security,
                        EnterTime = position.Time,
                        ExitTime = checkStopLossDay,
                        EnterPrice = position.Price,
                        ExitPrice = priceToCheck,
                        Quantity = position.Quantity,
                        ExitType = ExitType.Close,
                        Parameters = position.Parameters
                    };
                    Trades.Add(trade);
                    PortfolioEquity += trade.Value;
                }
            }
            foreach (var stopLossPosition in stopLossPositions)
            {
                Positions.Remove(stopLossPosition);
            }

            // normal close position
            var closeDate = time.Next(WeeklyTradeDay);
            if (closeDate > TestEnd || closeDate > EndOfData)
            {
                closeDate = EndOfData > TestEnd ? TestEnd : EndOfData;
                if (Positions.Count == 0 || Positions.Any(p => p.Time == closeDate))
                {
                    Positions.Clear();
                    return true;
                }
            }

            foreach (var position in Positions)
            {
                if (!DataCache.PriceCache.ContainsKey(closeDate))
                    throw new InvalidOperationException();
                if (position.Time >= closeDate)
                    throw new InvalidOperationException();

                var prices = SkipNoPriceDates(ref closeDate);
                if (prices == null)
                    return false;

                var sid = position.Security.Id;
                var closeDateWithPrice = closeDate;

                // try to fix price missing issue. if neither in db, use day before (up to 7 days)
                double price;
                if (!prices.ContainsKey(sid))
                {
                    var p = access.GetPrice(sid, closeDate.ToDateInt());
                    if (p == null)
                    {
                        p = ReverseNoPriceDate(sid, ref closeDateWithPrice);
                        if (p == null)
                        {
                            // no other way round!
                            throw new InvalidOperationException();
                        }
                    }
                    else
                    {
                        DataCache.PriceCache[closeDate][sid] = p;
                    }
                    price = p.AdjClose;
                }
                else
                {
                    price = prices[sid].AdjClose;
                }

                var trade = new Trade
                {
                    Security = position.Security,
                    EnterTime = position.Time,
                    ExitTime = closeDateWithPrice,
                    EnterPrice = position.Price,
                    ExitPrice = price,
                    Quantity = position.Quantity,
                    ExitType = ExitType.Close,
                    Parameters = position.Parameters
                };
                Trades.Add(trade);
                PortfolioEquity += trade.Value;
            }
            Positions.Clear();
            var snpPrice = DataCache.PriceCache[closeDate][snpSid].AdjClose;
            PortfolioStatuses.Add(new PortfolioStatus(closeDate, PortfolioEquity, snpPrice));
            return true;
        }

        private Price ReverseNoPriceDate(long sid, ref DateTime close)
        {
            var cache = DataCache.PriceCache;

            var shiftCounter = 0;
            while (true)
            {
                if (cache.ContainsKey(close))
                {
                    if (cache[close].ContainsKey(sid))
                        return cache[close][sid];
                    close = close.AddDays(-1);
                }
                shiftCounter++;
                if (shiftCounter > 7)
                {
                    return null;
                }
            }
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

        public string GetDataCriteriaInSql()
        {
            return "VOLUME > 500000 && ADJCLOSE > 10 && CLOSE > 10";
        }

        private bool SatisfyBuySell(Price p)
        {
            return p.Volume > 500000 && p.AdjClose > 10 && p.Close > 10;
        }

        public bool Next()
        {
            CurrentDate = CurrentDate.AddDays(1);
            return CurrentDate <= TestEnd || CurrentDate <= EndOfData;
        }

        public bool NextWeeklyTradeDay()
        {
            CurrentDate = CurrentDate.Next(WeeklyTradeDay);
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