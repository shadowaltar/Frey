using System;

namespace Trading.Common.Entities
{
    public class Price
    {
        public Security Security { get; set; }
        public DateTime At { get; set; }
        public TimeSpan Span { get; set; }

        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }

        public long Volume { get; set; }
    }
}