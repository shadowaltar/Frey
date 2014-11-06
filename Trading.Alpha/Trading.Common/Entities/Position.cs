using System;
using Trading.Common.Utils;

namespace Trading.Common.Entities
{
    public class Position
    {
        public Security Security { get; set; }
        public double Price { get; set; }
        public double Quantity { get; set; }
        public double Value { get { return Price * Quantity; } }
        public DateTime Time { get; set; }

        public double Parameter { get; set; }

        public override string ToString()
        {
            return string.Format("[{3}] {0}, ${1} x {2}", Security.Code, Price, Quantity, Time.IsoFormat());
        }
    }
}