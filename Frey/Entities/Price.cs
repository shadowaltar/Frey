using System;

namespace Automata.Entities
{
    public class Price
    {
        public Price()
        {
        }

        public Price(Price price)
        {
            Security = price.Security;
            Time = price.Time;
            Duration = price.Duration;
            Open = price.Open;
            High = price.High;
            Low = price.Low;
            Close = price.Close;
            AdjustedClose = price.AdjustedClose;
            Volume = price.Volume;
        }

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
            return string.Format("[{0}] {1}, {2}", Time, Security, Close);
        }
    }
}