using System;
using System.Collections.Generic;
using Automata.Entities;
using Automata.Mechanisms;

namespace Automata.Strategies
{
    public class ForexMACDCrossingStrategy : Strategy
    {
        public override bool IsTimeToStop { get; protected set; }

        public Dictionary<Security, List<Price>> PriceHistory { get; private set; }

        public override List<Order> GenerateOrders(HashSet<Price> prices, Portfolio portfolio, DateTime orderTime)
        {
            var expectedNextTimestamp = TradingScope.TickDuration;
            return null;
        }
    }
}