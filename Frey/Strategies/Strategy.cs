using Automata.Entities;
using Automata.Mechanisms;
using System;
using System.Collections.Generic;

namespace Automata.Strategies
{
    public abstract class Strategy
    {
        public virtual TimeScale TimeScale { get { return TimeScale.Daily; } }

        public abstract bool IsTimeToStop { get; protected set; }
        public ITradingScope TradingScope { get; set; }

        public virtual void Initialize()
        {
        }

        public abstract List<Order> GenerateOrders(HashSet<Price> prices, Portfolio portfolio, DateTime orderTime);
        protected abstract double ComputeQuantity(Portfolio portfolio, Security security, Price referencePrice);
    }
}