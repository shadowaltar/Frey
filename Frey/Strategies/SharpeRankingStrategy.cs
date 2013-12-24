using System;
using System.Collections.Generic;
using System.Linq;
using Automata.Core.Extensions;
using Automata.Core.Exceptions;
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
        public SharpeRankingStrategy(int periods = 20, int portfolioSize = 3)
        {
            PriceHistory = new Dictionary<Security, List<Price>>();
            Period = periods;
            PortfolioSize = portfolioSize;
        }

        public override bool IsTimeToStop { get; protected set; }

        private readonly Dictionary<Security, double> ranks = new Dictionary<Security, double>();
        public Dictionary<Security, List<Price>> PriceHistory { get; private set; }

        private DateTime initStartDate;

        private DateTime periodEnd;
        private DateTime dateCounter;
        private int counter = 0;

        public int Period { get; set; }
        public int PortfolioSize { get; set; }

        public override void Initialize()
        {
            initStartDate = TradingScope.Start.ClosestFuture(DayOfWeek.Tuesday);
            dateCounter = initStartDate;
            periodEnd = dateCounter.AddWeeks(4);
        }

        public override List<Order> GenerateOrders(HashSet<Price> data, List<Position> existingPositions)
        {
            // check if meets stop trading criteria
            if (data.Any(p => p.Time == TradingScope.End))
            {
                IsTimeToStop = true;
                // generate 'all close' orders
                return existingPositions.Select(p => Order.CreateToClose(p)).ToList();
            }

            counter++;
            var orders = new List<Order>();
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

            // approx 4 weeks later without considering holidays
            if (counter == Period)
            {
                var ranks = new List<Rank>();
                foreach (var security in PriceHistory.Keys)
                {
                    var prices = PriceHistory[security];
                    ranks.Add(ComputeRank(prices, security));
                }
                // var orderedRanks = ranks.OrderByDescending(r => r.RankValue).Take(PortfolioSize);
                var newOrders = new List<Order>();
                foreach (var rank in ranks.OrderByDescending(r => r.RankValue).Take(PortfolioSize))
                {
                    var security = rank.Security;
                    var lastPrice = PriceHistory[security].Last();
                    var q = ComputeQuantity(security, lastPrice);
                    newOrders.Add(new Order(security, Side.Long, lastPrice.AdjustedClose, q, 0));
                }
                orders.AddRange(newOrders);

                counter = 0;

                // check existing positions to see if already holding the security, or need to sell
                // those not in the ranks anymore.
                if (!existingPositions.IsNullOrEmpty())
                {
                    var closePositionOrders = new List<Order>();
                    foreach (var position in existingPositions)
                    {
                        var order = orders.FirstOrDefault(o => o.Security == position.Security); // maintains a hold
                        if (order != null)
                        {
                            if (order.Side == Side.Long || order.Side == Side.Hold)
                            {
                                order.Side = Side.Hold;
                            }
                            else if (order.Side == Side.Short)
                            {
                                throw new InvalidShortSellException();
                            }
                        }
                        else
                        {
                            closePositionOrders.Add(Order.CreateToClose(position));
                        }
                    }
                    orders.AddRange(closePositionOrders);
                }
                PriceHistory.Clear();

                Console.WriteLine(Utilities.BracketNow + " Orders Generated.");
            }
            return orders;
        }

        protected override double ComputeQuantity(Security security, Price referencePrice)
        {
            var a = 100000 * .02 / referencePrice.AdjustedClose;
            if (a < 100)
                return 100;
            var b = a % 100;
            return a - b;
        }

        private Rank ComputeRank(List<Price> prices, Security security)
        {
            if (prices.IsNullOrEmpty())
                return null;
            if (prices.Count == 1)
                return new Rank(security) { Return = 0, Sigma = 0 };

            var startingPrice = prices.First().AdjustedClose;
            var holdingPeriodReturn = (prices.Last().AdjustedClose - startingPrice) / startingPrice;
            var returns = new List<double>();

            for (var i = 1; i < prices.Count; i++)
            {
                var prevClose = prices[i - 1].AdjustedClose;
                returns.Add((prices[i].AdjustedClose - prevClose) / prevClose + 1);
            }
            var s = new Rank(security)
            {
                Return = holdingPeriodReturn,
                Sigma = returns.StandardDeviation()
            };
            s.SharpeRatio = s.Return / s.Sigma;
            s.RankValue = s.Return;
            return s;
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

            public double RankValue { get; set; }
        }
    }
}