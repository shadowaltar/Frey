using System;
using System.Collections.Generic;
using System.Linq;
using Automata.Core.Extensions;
using Automata.Entities;
using Automata.Mechanisms;
using System.Collections.Concurrent;
using Automata.Core;
using MathNet.Numerics.Statistics;

namespace Automata.Strategies
{
    /// <summary>
    /// Find highest ranking equities, hold and sell after 4 weeks or if hit stoploss (holiday excluded).
    /// Ranking criteria: (from high to low)
    ///     rank = geometric average daily return (4W) / standard deviation of the daily returns (4W).
    /// Stoploss criteria: (assuming init prices is p0)
    ///     sl = 0.85*p0.
    /// </summary>
    public class SharpeRankingStrategy : Strategy
    {
        public SharpeRankingStrategy()
        {
            PriceHistory = new Dictionary<Security, List<Price>>();
        }

        public override bool IsTimeToStop { get { return false; } }

        private readonly Dictionary<Security, double> ranks = new Dictionary<Security, double>();
        public Dictionary<Security, List<Price>> PriceHistory { get; private set; }

        private DateTime initStartDate;

        private DateTime periodEnd;
        private DateTime dateCounter;
        private int counter = 0;

        public override void Initialize()
        {
            initStartDate = TradingScope.Start.ClosestFuture(DayOfWeek.Tuesday);
            dateCounter = initStartDate;
            periodEnd = dateCounter.AddWeeks(4);
        }

        public override List<Order> GenerateOrders(HashSet<Price> data, List<Position> existingPositions)
        {
            counter++;
            // save the incoming data into cache
            foreach (var price in data)
            {
                var security = price.Security;
                if (!PriceHistory.ContainsKey(security))
                {
                    PriceHistory[security] = new List<Price>();
                }
                PriceHistory[security].Add(price);
            }

            if (counter == 20) // approx 4 weeks later without considering holidays
            {
                List<Order> orders = new List<Order>();
                List<Price> prices = null;
                var ranks = new List<Rank>();
                foreach (var security in PriceHistory.Keys)
                {
                    prices = PriceHistory[security];
                    ranks.Add(ComputeRank(prices, security));
                }
                var orders = ranks.OrderByDescending(r => r.SharpeRatio)
                    .Take(3).Select(r => new Order(r.Security, Side.Long, 100));
                PriceHistory.Clear();

                counter = 0;
            }
            //    Console.WriteLine(Utilities.BracketNow + " Generating Entry Orders.");
            return null;
        }

        private Rank ComputeRank(List<Price> prices, Security security)
        {
            if (prices.IsNullOrEmpty())
                return null;
            if (prices.Count == 1)
                return new Rank(security) { Return = 0, Sigma = 0 };

            var returns = new List<double>();
            for (var i = 1; i < prices.Count; i++)
            {
                var prevClose = prices[i - 1].AdjustedClose;
                returns.Add((prices[i].AdjustedClose - prevClose) / prevClose);
            }
            var s = new Rank(security) { Return = returns.Average(), Sigma = returns.StandardDeviation() };
            s.SharpeRatio = s.Return / s.Sigma;
            return s;
        }

        public override List<Order> GenerateExits(HashSet<Price> data, List<Position> existingPositions)
        {
            Console.WriteLine(Utilities.BracketNow + " Generating Exit Orders.");
            return null;
        }

        public class Rank
        {
            public Rank(Security security)
            {
                Security = security;
            }

            public Security Security { get; private set; }

            public double Return { get; set; }
            public double Sigma { get; set; }

            public double SharpeRatio { get; set; }
        }
    }
}