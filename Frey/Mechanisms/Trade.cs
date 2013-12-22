using Automata.Entities;

namespace Automata.Mechanisms
{
    public class Trade
    {
        public Position Position { get; set; }
        public Order ExitOrder { get; set; }
        public double Profit { get; set; }
    }
}