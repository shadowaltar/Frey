using System;

namespace Automata.Entities
{
    public class Price
    {
        public Security Security { get; set; }
        public DateTime Time { get; set; }
        public TimeSpan Duration { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public double AdjustedClose { get; set; }
        public double Volume { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}] {1}, {2}", Time, Security, AdjustedClose);
        }
    }
}