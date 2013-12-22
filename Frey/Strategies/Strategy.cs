using System.Collections.Generic;
using Automata.Entities;
using Automata.Mechanisms;

namespace Automata.Strategies
{
    public abstract class Strategy
    {
        public virtual TimeScale TimeScale { get { return TimeScale.Daily; } }

        public abstract bool IsTimeToStop { get; }
        public ITradingScope TradingScope { get; set; }

        public abstract List<Order> GenerateExits(PriceCache data, List<Position> existingPositions);

        public abstract List<Order> GenerateEntries(PriceCache data, List<Position> existingPositions);
    }
}