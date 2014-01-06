﻿using Automata.Entities;
using System;
using System.Collections.Generic;

namespace Automata.Mechanisms
{
    public interface ITradingScope
    {
        DateTime Start { get; set; }
        DateTime End { get; set; }
        TimeSpan PriceInterval { get; set; }
        List<Security> Securities { get; set; }
    }
}