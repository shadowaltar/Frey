using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Automata.Core.Exceptions;
using Automata.Core.Extensions;
using Automata.Entities;
using Automata.Mechanisms;
using Automata.Quantitatives.Indicators;

namespace Automata.Strategies
{
    public class ForexMACDCrossingStrategy : Strategy
    {
        public override bool IsTimeToStop { get; protected set; }

        public Dictionary<Security, List<Price>> PriceHistory { get; private set; }

        private MACD macd;

        public override void Initialize()
        {
            macd = Indicators[0] as MACD;
            if (macd == null)
                throw new InvalidStrategyBehaviorException();
        }

        private HashSet<Price> lastPrices = new HashSet<Price>();

        public override List<Order> GenerateOrders(HashSet<Price> prices, Portfolio portfolio, DateTime orderTime)
        {
            if (prices.IsNullOrEmpty())
                return null;


            foreach (var price in prices)
            {
                macd.Compute(price);
                macd.HistogramValues.LastOrDefault();
            }
            return null;
        }
    }
}