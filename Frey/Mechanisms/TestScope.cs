using System;
using System.Collections.Generic;
using Automata.Core.Extensions;
using Automata.Entities;
using MathNet.Numerics;

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
        public TimeSpan SourcePriceDuration { get; set; }
        public TimeSpan TargetPriceDuration { get; set; }

        public List<Country> Countries { get; set; }
        public List<Exchange> Exchanges { get; set; }
        public List<Security> Securities { get; set; }
        public double LeverageMultiplier { get; set; }
        public PriceSourceType PriceSourceType { get; set; }
        public TimeZoneType PriceTimeZoneType { get; set; }
        public double MarginRatio { get { return LeverageMultiplier.ApproxEqualTo(0) ? double.NaN : 1 / LeverageMultiplier; } }
        public PriceType DefaultIndicatorPriceType { get; set; }
    }
}