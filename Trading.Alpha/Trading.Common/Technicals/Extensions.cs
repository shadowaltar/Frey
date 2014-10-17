using System.Collections.Generic;
using System.Linq;
using Trading.Common.Entities;
using Trading.Common.Technicals.Entities;

namespace Trading.Common.Technicals
{
    public static class Extensions
    {
        /// <summary>
        /// Fast MovingAverage LINQ Extension method provided for example purposes on an AS IS BASIS ONLY. ACCURACY OF CALCULATION NOT GAURANTEED
        /// </summary>
        public static IEnumerable<double> MovingAverage(this IEnumerable<double> inputs, int period)
        {
            var ma = new MovingAverage(period);
            foreach (var item in inputs)
            {
                ma.Push(item);
                yield return ma.Current;
            }
        }

        /// <summary>
        /// Fast Macd LINQ Extension method provided for example purposes on an AS IS BASIS ONLY. ACCURACY OF CALCULATION NOT GAURANTEED
        /// </summary>
        public static IEnumerable<MacdPoint> Macd(this IEnumerable<double> inputs, int slowPeriod, int fastPeriod, int signalPeriod)
        {
            var maSlow = new MovingAverage(slowPeriod);
            var maFast = new MovingAverage(fastPeriod);
            var maSignal = new MovingAverage(signalPeriod);

            foreach (var input in inputs)
            {
                var maSlowValue = maSlow.Push(input).Current;
                var maFastValue = maFast.Push(input).Current;
                double macd = double.IsNaN(maSlowValue) || double.IsNaN(maFastValue)
                    ? double.NaN : maSlowValue - maFastValue;
                double signalLine = double.IsNaN(macd) ? double.NaN : maSignal.Push(macd).Current;
                double divergence = double.IsNaN(macd) || double.IsNaN(signalLine) ? double.NaN : macd - signalLine;

                yield return new MacdPoint { Macd = macd, Signal = signalLine, Divergence = divergence };
            }
        }

        /// <summary>
        /// Fast Rsi LINQ Extension method provided for example purposes on an AS IS BASIS ONLY. ACCURACY OF CALCULATION NOT GAURANTEED
        /// </summary>
        public static IEnumerable<double> Rsi(this IEnumerable<PriceBar> inputs, int period)
        {
            var averageGain = new MovingAverage(period);
            var averageLoss = new MovingAverage(period);

            var previous = inputs.First();
            foreach (var item in inputs.Skip(1))
            {
                double gain = item.Close > previous.Close ? item.Close - previous.Close : 0.0d;
                double loss = previous.Close > item.Close ? previous.Close - item.Close : 0.0d;

                averageGain.Push(gain);
                averageLoss.Push(loss);

                double relativeStrength = double.IsNaN(averageGain.Current) || double.IsNaN(averageLoss.Current)
                    ? double.NaN
                    : averageGain.Current / averageLoss.Current;

                previous = item;
                yield return double.IsNaN(relativeStrength) ? double.NaN : 100d - (100d / (1d + relativeStrength));
            }
        }
    }
}