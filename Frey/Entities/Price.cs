using Automata.Core;
using System;
using System.Collections.Generic;
using System.Linq;

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
            Start = price.Start;
            Duration = price.Duration;
            Open = price.Open;
            High = price.High;
            Low = price.Low;
            Close = price.Close;
            AdjustedClose = price.AdjustedClose;
            Volume = price.Volume;
        }

        public Security Security { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get { return Start + Duration; } }
        public TimeSpan Duration { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public double AdjustedClose { get; set; }
        public double Volume { get; set; }

        public static double HighestHigh(IEnumerable<Price> prices)
        {
            return prices.Max(p => p.High);
        }

        public static double LowestLow(IEnumerable<Price> prices)
        {
            return prices.Min(p => p.Low);
        }

        public static Price Combine(IEnumerable<Price> prices, DateTime startTime, TimeSpan newDuration)
        {
            Price result = null;
            var close = 0d;
            var adjClose = 0d;
            foreach (var price in prices)
            {
                if (result == null)
                {
                    result = new Price(price)
                    {
                        Start = startTime,
                        Duration = newDuration,
                        Volume = 0
                    };
                }
                if (result.High < price.High)
                    result.High = price.High;
                if (result.Low > price.Low)
                    result.Low = price.Low;

                result.Volume += price.Volume;
                close = price.Close;
                adjClose = price.AdjustedClose;
            }
            if (result != null)
            {
                result.Close = close;
                result.AdjustedClose = adjClose;
            }
            return result;
        }

        public override string ToString()
        {
            return string.Format("{0} {1}, {2}", Start.PrintBracket(), Security, Close);
        }
    }
}