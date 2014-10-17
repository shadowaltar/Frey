using System;

namespace Trading.Common.Entities
{
    public class Price
    {
        public Security Security { get; set; }
        public DateTime At { get; set; }
        public TimeSpan Span { get; set; }

        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }

        public long Volume { get; set; }
    }
}