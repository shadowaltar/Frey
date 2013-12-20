﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Automata.Entities
{
    public class ExchangeTradable : Security
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public Exchange Exchange { get; set; }
    }
}