using System;
using System.Collections.Generic;
using System.Linq;
using Automata.Core.Extensions;
using Automata.Entities;
using Automata.Mechanisms;

namespace Automata.Strategies
{
    public class ForexMACDCrossingStrategy : Strategy
    {
        public override bool IsTimeToStop { get; protected set; }

        public Dictionary<Security, List<Price>> PriceHistory { get; private set; }

        private DateTime currentTimestamp = DateTime.MinValue;
        private DateTime expectedNextTimestamp = DateTime.MinValue;

        public override List<Order> GenerateOrders(HashSet<Price> prices, Portfolio portfolio, DateTime orderTime)
        {
            if (prices.IsNullOrEmpty())
                return null;
            if (currentTimestamp == DateTime.MinValue)
                currentTimestamp = prices.First().Time;

            var expectedNextTimestamp = currentTimestamp + TradingScope.TickDuration;
            foreach (var price in prices)
            {

            }
            return null;
        }
    }
}