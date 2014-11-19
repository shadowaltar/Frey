using System;

namespace Trading.Common.Entities
{
    public class Price
    {
        public long SecId { get; set; }

        public DateTime Time { get; set; }

        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public double AdjClose { get; set; }

        public long Volume { get; set; }
    }
}