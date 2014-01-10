using Automata.Core.Exceptions;
using Automata.Core.Extensions;
using Automata.Entities;
using Automata.Mechanisms.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Automata.Quantitatives.Indicators
{
    public class MACD : Indicator
    {
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

        public PriceType PriceType { get; private set; }

        public int SignalPeriods { get; private set; }
        public int EMAFastPeriods { get; private set; }
        public int EMASlowPeriods { get; private set; }

        public List<TimeValue> SignalValues { get { return macdEmaValues; } }
        public List<TimeValue> MACDValues { get { return macdValues; } }
        public List<TimeValue> HistogramValues { get { return histogramValues; } }

        private readonly List<TimeValue> macdValues = new List<TimeValue>();
        private readonly List<TimeValue> histogramValues = new List<TimeValue>();
        private readonly double signalPeriodFactor;
        private readonly double fastPeriodFactor;
        private readonly double slowPeriodFactor;
        private readonly List<TimeValue> fastEmaValues = new List<TimeValue>();
        private readonly List<TimeValue> slowEmaValues = new List<TimeValue>();
        private readonly List<TimeValue> macdEmaValues = new List<TimeValue>();
        private readonly List<TimeValue> prices;
        private readonly int expectedPriceCount;

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
            var value = price.ValueOf(PriceType);
            if (prices.Count == expectedPriceCount && isAddNew)
            {
                prices.RemoveAt(0);
            }
            prices.AddOrReplaceLast(new TimeValue(price.Time, value), isAddNew);

            var lastValue = fastEmaValues.LastOrDefault();
            var fastEma = IndicatorMaths.CalculateEMA(value, fastPeriodFactor,
                lastValue == null ? value : lastValue.Value);
            fastEmaValues.AddOrReplaceLast(new TimeValue(price.Time, fastEma), isAddNew);

            lastValue = slowEmaValues.LastOrDefault();
            var slowEma = IndicatorMaths.CalculateEMA(value, slowPeriodFactor,
                lastValue == null ? value : lastValue.Value);
            slowEmaValues.AddOrReplaceLast(new TimeValue(price.Time, slowEma), isAddNew);

            var macd = fastEma - slowEma;
            MACDValues.AddOrReplaceLast(new TimeValue(price.Time, macd), isAddNew);

            lastValue = SignalValues.LastOrDefault();
            var macdEma = IndicatorMaths.CalculateEMA(macd, signalPeriodFactor,
                lastValue == null ? 0d : lastValue.Value);
            SignalValues.AddOrReplaceLast(new TimeValue(price.Time, macdEma), isAddNew);

            HistogramValues.AddOrReplaceLast(new TimeValue(price.Time, macd - macdEma), isAddNew);
        }
    }
}
