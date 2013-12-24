using System.Collections.Generic;
using Automata.Entities;
using Automata.Mechanisms;
using System.Collections.Concurrent;

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

        public abstract List<Order> GenerateOrders(HashSet<Price> data, List<Position> existingPositions);
        protected abstract double ComputeQuantity(Security security, Price referencePrice);
    }
}