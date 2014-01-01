using System.Collections.Generic;

namespace Automata.Mechanisms
{
    public class Portfolio : List<Position>
    {
        public CashPosition CashPosition { get; set; }
    }
}