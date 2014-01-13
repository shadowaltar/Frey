using System;
using Automata.Entities;

namespace Automata.Mechanisms
{
    public class DailyTradingSession
    {
        public MarketType MarketType { get; set; }
        public City City { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}