using Automata.Core;
using Automata.Core.Exceptions;
using Automata.Core.Extensions;
using Automata.Entities;
using Automata.Mechanisms;
using MathNet.Numerics.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public Dictionary<Security, List<Price>> PriceHistory { get; private set; }

        private int counter;

        public int Period { get; set; }
        public int PortfolioSize { get; set; }

        public override void Initialize()
        {
        }

        public override List<Order> GenerateOrders(HashSet<Price> prices, Portfolio portfolio, DateTime orderTime)
        {
            counter++;
            var orders = new List<Order>();
            // save the incoming prices into cache
            foreach (var price in prices)
            {
                var security = price.Security;
                if (!PriceHistory.ContainsKey(security))
                {
                    PriceHistory[security] = new List<Price>();
                }
                PriceHistory[security].Add(price);
            }

            // close position if hits the stoploss price
            foreach (var position in portfolio)
            {
                var price = prices.FirstOrDefault(p => p.Security == position.Security);
                if (price != null)
                {
                    if (price.Close <= position.Order.StopLossPrice)
                    {
                        orders.Add(Order.CreateToClose(position, orderTime, true));
                    }
                }
            }

            // approx 4 weeks later without considering holidays
            if (counter == Period)
            {
                var ranks = new List<Rank>();
                foreach (var security in PriceHistory.Keys)
                {
                    ranks.Add(ComputeRank(PriceHistory[security], security));
                }

                var newOrders = new List<Order>();
                foreach (var rank in ranks.OrderByDescending(r => r.RankValue).Take(PortfolioSize))
                {
                    if (rank.SharpeRatio > 0)
                    {
                        var security = rank.Security;
                        var lastPrice = PriceHistory[security].Last();
                        var q = ComputeQuantity(portfolio, lastPrice);
                        newOrders.Add(new Order(security, Side.Long, lastPrice.Close, q, ComputeStopLoss(lastPrice),
                            orderTime));
                    }
                }
                orders.AddRange(newOrders);

                counter = 0;

                // check existing positions to see if already holding the security, or need to sell
                // those not in the ranks anymore.
                if (!portfolio.IsNullOrEmpty())
                {
                    var closePositionOrders = new List<Order>();
                    foreach (var position in portfolio)
                    {
                        // must not be the closing order already decided by the stoploss logic
                        var order = orders.FirstOrDefault(o => o.Security == position.Security);
                        if (order == null)
                        {
                            closePositionOrders.Add(Order.CreateToClose(position, orderTime));
                        }
                        else if (!order.IsStopLossClosing)
                        {
                            if (order.Side == Side.Long || order.Side == Side.Hold)
                            {
                                // maintains a hold
                                order.Side = Side.Hold;
                                orders.Remove(order);
                            }
                            else if (order.Side == Side.Short)
                            {
                                throw new InvalidShortSellException();
                            }
                        }
                    }
                    orders.AddRange(closePositionOrders);
                }
                PriceHistory.Clear();
            }
            foreach (var order in orders)
            {
                Console.WriteLine(Utilities.BracketNow + " New Order: " + order);
            }
            return orders;
        }

        private double ComputeStopLoss(Price lastPrice)
        {
            return lastPrice.Close * .80; // stoploss is when drawdown = 10%
        }

        private static double ComputeQuantity(Portfolio portfolio, Price referencePrice)
        {
            var a = portfolio.CashPosition.Value * .15 / referencePrice.Close;
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

            var startingPrice = prices.First().Close;
            var holdingPeriodReturn = (prices.Last().Close - startingPrice) / startingPrice;
            var returns = new List<double>();

            for (var i = 1; i < prices.Count; i++)
            {
                var prevClose = prices[i - 1].Close;
                returns.Add((prices[i].Close - prevClose) / prevClose + 1);
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