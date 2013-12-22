using System;
using System.Collections.Generic;
using Automata.Entities;

namespace Automata.Mechanisms
{
    public class TestScope : ITradingScope
    {
        public TestScope()
        {
            Countries = new List<Country>();
            Exchanges = new List<Exchange>();
            Securities = new List<Security>();
        }

        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public TimeSpan PriceInterval { get; set; }

        public List<Country> Countries { get; set; }
        public List<Exchange> Exchanges { get; set; }
        public List<Security> Securities { get; set; }
    }
}