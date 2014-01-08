using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Automata.Core.Exceptions;
using Automata.Entities;
using Automata.Mechanisms.Utils;

namespace Automata.Quantitatives.Indicators
{
    public class MACD : Indicator
    {
        public PriceType PriceType { get; private set; }

        public int SignalPeriods { get; private set; }
        private readonly double signalPeriodFactor;
        public int EMAFastPeriods { get; private set; }
        private readonly double fastPeriodFactor;
        public int EMASlowPeriods { get; private set; }
        private readonly double slowPeriodFactor;

        public MACD(int signalPeriods = 9, int fastPeriods = 12, int slowPeriods = 26, PriceType priceType = PriceType.Close)
        {
            if (signalPeriods <= 0)
                throw new IndicatorArgumentException();
            if (fastPeriods <= 0)
                throw new IndicatorArgumentException();
            if (slowPeriods <= 0)
                throw new IndicatorArgumentException();
            if (fastPeriods > slowPeriods)
                throw new IndicatorArgumentException();

            Name = "Moving Average Convergence/Divergence";

            SignalPeriods = signalPeriods;
            EMAFastPeriods = fastPeriods;
            EMASlowPeriods = slowPeriods;
            PriceType = priceType;

            fastPeriodFactor = 2 / (double)(fastPeriods + 1);
            slowPeriodFactor = 2 / (double)(slowPeriods + 1);
            signalPeriodFactor = 2 / (double)(signalPeriods + 1);

            expectedPriceCount = Math.Max(slowPeriods, signalPeriods);

            prices = new List<TimeValue>(expectedPriceCount);
        }

        public List<TimeValue> SignalValues { get { return macdEmaValues; } }

        private readonly List<TimeValue> macdValues = new List<TimeValue>();
        public List<TimeValue> MACDValues { get { return macdValues; } }

        private readonly List<TimeValue> histogramValues = new List<TimeValue>();
        public List<TimeValue> HistogramValues { get { return histogramValues; } }

        private readonly List<TimeValue> fastEmaValues = new List<TimeValue>();
        private readonly List<TimeValue> slowEmaValues = new List<TimeValue>();
        private readonly List<TimeValue> macdEmaValues = new List<TimeValue>();
        private readonly List<TimeValue> prices;
        private readonly int expectedPriceCount;

        public void Compute(Price price)
        {
            var value = price.ValueOf(PriceType);
            if (prices.Count > expectedPriceCount)
            {
                prices.RemoveAt(0);
                prices.Add(new TimeValue(price.Time, value));
            }

            var lastValue = fastEmaValues.LastOrDefault();
            var fastEma = IndicatorMaths.CalculateEMA(value, fastPeriodFactor,
                lastValue == null ? 0d : lastValue.Value);
            fastEmaValues.Add(new TimeValue(price.Time, fastEma));

            lastValue = slowEmaValues.LastOrDefault();
            var slowEma = IndicatorMaths.CalculateEMA(value, slowPeriodFactor,
                lastValue == null ? 0d : lastValue.Value);
            slowEmaValues.Add(new TimeValue(price.Time, slowEma));

            var macd = fastEma - slowEma;
            MACDValues.Add(new TimeValue(price.Time, macd));

            lastValue = SignalValues.LastOrDefault();
            var macdEma = IndicatorMaths.CalculateEMA(macd, signalPeriodFactor,
                lastValue == null ? 0d : lastValue.Value);
            SignalValues.Add(new TimeValue(price.Time, macdEma));

            HistogramValues.Add(new TimeValue(price.Time, macd - macdEma));
        }
    }
}
