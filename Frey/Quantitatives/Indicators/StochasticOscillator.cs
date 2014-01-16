using System;
using System.Collections.Generic;
using System.Linq;
using Automata.Core.Extensions;
using Automata.Entities;
using Automata.Mechanisms.Utils;

namespace Automata.Quantitatives.Indicators
{
    /// <summary>
    /// Stochastic Oscillator.
    /// Fast %K = (close - min(low))/(max(high)-min(low)) * 100. Max & min are values over 'kPeriods'.
    /// Slow %K = n-period SMA of Fast %K. N is 'slowingPeriods'.
    /// Slow %D = n-period SMA of Slow %K. N is 'dPeriods'.
    /// Output is Slow %K and Slow %D.
    /// </summary>
    public class StochasticOscillator : Indicator
    {
        public int KPeriods { get; set; }
        public int DPeriods { get; set; }
        public int SlowingPeriods { get; set; }
        public PriceType PriceType { get; set; }
        public List<TimeValue> KValues { get { return slowKValues; } }
        public List<TimeValue> DValues { get { return slowDValues; } }

        private readonly List<double> highs;
        private readonly List<double> lows;
        private readonly List<double> fastKValues;

        private readonly List<TimeValue> slowKValues = new List<TimeValue>();
        private readonly List<TimeValue> slowDValues = new List<TimeValue>();

        private double highestHigh;
        private double lowestLow;

        public StochasticOscillator(int kPeriods = 8,
            int dPeriods = 3, int slowingPeriods = 3, PriceType priceType = PriceType.Close)
        {
            KPeriods = kPeriods;
            DPeriods = dPeriods;
            SlowingPeriods = slowingPeriods;
            PriceType = priceType;
            Name = "Stochastic Oscillator";

            highs = new List<double>(kPeriods);
            lows = new List<double>(kPeriods);
            fastKValues = new List<double>(kPeriods);
        }

        public override void ComputeNext(Price price)
        {
            Compute(price, true);
        }

        public override void ComputeCurrent(Price price)
        {
            Compute(price, false);
        }

        private void Compute(Price price, bool isAddNew)
        {
            var close = price.Close;

            if (highs.Count == KPeriods && isAddNew)
            {
                highs.RemoveAt(0);
                lows.RemoveAt(0);
                fastKValues.RemoveAt(0);
            }

            highs.AddOrReplaceLast(price.High, isAddNew);
            lows.AddOrReplaceLast(price.Low, isAddNew);
            highestHigh = highs.Max();
            lowestLow = lows.Min();
            var k = (close - lowestLow) / (highestHigh - lowestLow) * 100;
            fastKValues.AddOrReplaceLast(k, isAddNew);

            var slowK = fastKValues.TailAverage(SlowingPeriods);
            slowKValues.AddOrReplaceLast(new TimeValue(price.Start, slowK), isAddNew);

            var slowD = slowKValues.TailAverage(DPeriods);
            slowDValues.AddOrReplaceLast(new TimeValue(price.Start, slowD), isAddNew);
        }
    }
}