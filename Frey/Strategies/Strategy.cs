using System.Collections.Generic;
using Automata.Entities;
using Automata.Mechanisms;
using System.Collections.Concurrent;

namespace Automata.Strategies
{
    public abstract class Strategy
    {
        public virtual TimeScale TimeScale { get { return TimeScale.Daily; } }

        public abstract bool IsTimeToStop { get; }
        public ITradingScope TradingScope { get; set; }

        public virtual void Initialize()
        {
        }

        public abstract List<Order> GenerateExits(HashSet<Price> data, List<Position> existingPositions);

        public abstract List<Order> GenerateOrders(HashSet<Price> data, List<Position> existingPositions);
    }
}