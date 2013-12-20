using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Automata.Entities
{
    public class Exchange
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Mic { get; set; }

        public Country Country { get; set; }
    }
}
