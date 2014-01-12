using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using Automata.Core;
using Automata.Core.Exceptions;
using Automata.Core.Extensions;
using Automata.Entities;
using Automata.Mechanisms;
using Automata.Mechanisms.Utils;
using Automata.Quantitatives.Indicators;

namespace Automata.Strategies
{
    public class ForexStochasticOscillatorStrategy : Strategy
    {
        public override bool IsTimeToStop { get; protected set; }

        public Dictionary<Security, List<Price>> PriceHistory { get; private set; }

        private MACD macd;
        private StochasticOscillator sto;

        private int tickCounter = 0;
        private DateTime lastPriceTime = DateTime.MinValue;

        public override void Initialize()
        {
            macd = Indicators[0] as MACD;
            if (macd == null)
                throw new InvalidStrategyBehaviorException();
            sto = Indicators[1] as StochasticOscillator;
            if (sto == null)
                throw new InvalidStrategyBehaviorException();
        }

        private HashSet<Price> lastPrices = new HashSet<Price>();

        public override List<Order> GenerateOrders(HashSet<Price> prices, Portfolio portfolio, DateTime orderTime)
        {
            if (prices.IsNullOrEmpty())
                return null;

            tickCounter++;
            
            var orders = new List<Order>();
            foreach (var price in prices)
            {
                macd.ComputeNext(price);
                sto.ComputeNext(price);

                // don't trade when near market open
                if (NearMarketOpen(price.Time))
                    continue;

                // close all positions near the market closing
                if (NearMarketClose(price.Time))
                {
                    foreach (var position in portfolio)
                    {
                        Order.CreateToClose(position, orderTime);
                    }
                }
            }

            lastPriceTime = orderTime;
            return null;
        }

        private bool NearMarketClose(DateTime time)
        {
            return time.DayOfWeek == DayOfWeek.Saturday && time.Hour > 14;
        }

        private bool NearMarketOpen(DateTime time)
        {
            return time.DayOfWeek == DayOfWeek.Monday && time.Hour < 16;
        }
    }
}