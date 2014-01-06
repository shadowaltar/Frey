using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Automata.Core.Exceptions;
using Automata.Entities;
using Automata.Mechanisms;

namespace Automata.Strategies
{
    public class PairTradingStrategy : Strategy
    {
        public override bool IsTimeToStop { get; protected set; }

        public override void Initialize()
        {
            if (TradingScope == null)
                throw new InvalidStrategyBehaviorException();
            if (TradingScope.Securities.Count != 2)
                throw new InvalidStrategyBehaviorException();
            securityOne = TradingScope.Securities[0];
            securityTwo = TradingScope.Securities[1];
        }

        private Security securityOne;
        private Security securityTwo;

        public override List<Order> GenerateOrders(HashSet<Price> prices, Portfolio portfolio, DateTime orderTime)
        {
            var results = new List<Order>();
            var priceOne = prices.FirstOrDefault(p => p.Security == securityOne);
            var priceTwo = prices.FirstOrDefault(p => p.Security == securityTwo);
            if (priceOne != null && priceTwo != null)
            {
                var positionOne = portfolio[securityOne];
                var positionTwo = portfolio[securityTwo];

                // p2>p1, long s1 short s2
                if (priceOne.Close > priceTwo.Close)
                {
                    if (positionOne == null || positionOne.Side == Side.Short)
                    {
                        // close s1 posi if shorting
                        if (positionOne != null && positionOne.Side == Side.Short)
                        {
                            var closeOrder = Order.CreateToClose(positionOne, orderTime);
                            results.Add(closeOrder);
                        }
                        // long s1
                        results.Add(new Order(securityOne, Side.Long, OrderType.Market, priceOne.Close,
                            ComputeQuantity(portfolio, priceOne), ComputeStopLoss(), orderTime));
                    }
                    if (positionTwo == null || positionTwo.Side == Side.Long)
                    {
                        // close s2 posi if longing
                        if (positionTwo != null && positionTwo.Side == Side.Long)
                        {
                            var closeOrder = Order.CreateToClose(positionTwo, orderTime);
                            results.Add(closeOrder);
                        }
                        // short s2
                        results.Add(new Order(securityTwo, Side.Short, OrderType.Market, priceTwo.Close,
                            ComputeQuantity(portfolio, priceTwo), ComputeStopLoss(), orderTime));
                    }
                }

                // p1>p2, long s2 short s1
                if (priceTwo.Close > priceOne.Close)
                {
                    if (positionTwo == null || positionTwo.Side == Side.Short)
                    {
                        // close s2 posi if it is shorting
                        if (positionTwo != null && positionTwo.Side == Side.Short)
                        {
                            var closeOrder = Order.CreateToClose(positionTwo, orderTime);
                            results.Add(closeOrder);
                        }
                        // long s2
                        results.Add(new Order(securityTwo, Side.Long, OrderType.Market, priceTwo.Close,
                            ComputeQuantity(portfolio, priceTwo), ComputeStopLoss(), orderTime));
                    }
                    if (positionOne == null || positionOne.Side == Side.Long)
                    {
                        // close s2 posi if it is longing
                        if (positionOne != null && positionOne.Side == Side.Long)
                        {
                            var closeOrder = Order.CreateToClose(positionOne, orderTime);
                            results.Add(closeOrder);
                        }
                        // short s1
                        results.Add(new Order(securityOne, Side.Short, OrderType.Market, priceOne.Close,
                            ComputeQuantity(portfolio, priceOne), ComputeStopLoss(), orderTime));
                    }
                }
            }
            return results;
        }

        private double ComputeStopLoss()
        {
            return 0;
        }

        private static double ComputeQuantity(Portfolio portfolio, Price referencePrice)
        {
            var a = portfolio.CashPosition.Value * .25 / referencePrice.Close;
            if (a < 100)
                return 100;
            var b = a % 100;
            return a - b;
        }
    }
}
