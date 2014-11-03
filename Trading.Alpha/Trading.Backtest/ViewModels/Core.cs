using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CsvHelper;
using Trading.Backtest.Data;
using Trading.Common;
using Trading.Common.Entities;
using Trading.Common.Utils;

namespace Trading.Backtest.ViewModels
{
    public class Core
    {
        public HashSet<Position> Positions { get; set; }
        public double PortfolioAmount { get; set; }
        public double RemainingCash { get; set; }

        public int LongPositionCount { get; private set; }
        public int ShortPositionCount { get; private set; }

        public void EnterPositions(DateTime start)
        {
            var prices = DataCache.PriceCache;
            var dateOnePrices = prices[start];
            while (dateOnePrices.Count == 0)
            {
                Console.WriteLine("No security has price on #1 " + start.IsoFormat() + " so use next date.");
                start = start.AddDays(1);
                if (prices.ContainsKey(start))
                    dateOnePrices = prices[start];
            }
            var excludedDateOnePrices = dateOnePrices.Where(p => !SatisfyBuySell(p.Value))
                .Select(p => p.Key).ToHashSet();

            var tradeDate = start.Next(DayOfWeek.Tuesday);
            var dateTwoPrices = prices[tradeDate];
            while (dateTwoPrices.Count == 0)
            {
                Console.WriteLine("No security has price on #2 " + tradeDate.IsoFormat() + " so use next date.");
                tradeDate = tradeDate.AddDays(1);
                if (prices.ContainsKey(tradeDate))
                    dateTwoPrices = prices[tradeDate];
            }

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

            foreach (var pair in top10)
            {
                var sid = pair.Key;
                var newPosition = new Position();
                newPosition.Price = dateTwoPrices[sid].AdjClose;
                newPosition.Time = tradeDate;
                var q = Math.Floor(PortfolioAmount / 10 / newPosition.Price);
                if (q < 1)
                    continue;
                newPosition.Quantity = q;
                newPosition.Security = DataCache.SecurityCache[sid];
                Positions.Add(newPosition);
            }

            // report
            var resultPath = Path.Combine(Constants.LogsDirectory,
                "Positions[" + tradeDate.IsoFormat() + "].csv");
            using (var writer = new StreamWriter(resultPath))
            using (var csv = new CsvWriter(writer))
            {
                csv.WriteRecords(Positions);
            }
            Process.Start(resultPath);


        }

        public void ExitPositions(DateTime time)
        {
            throw new NotImplementedException();
        }

        public void ExitPositions()
        {
            throw new NotImplementedException();
        }

        private bool SatisfyBuySell(Price p)
        {
            return p.Volume > 200000 && p.AdjClose > 10 && p.AdjClose < 100;
        }
    }
}