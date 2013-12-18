using System;
using System.Collections.Generic;
using Automata.Entities;

namespace Automata.Mechanisms
{
    public class TestScope
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public TimeSpan TickDuration { get; set; }

        public string Market { get; set; }
        public List<Security> Securities { get; set; }
    }
}