using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Automata.Quantitatives
{
    public class Bins : List<Bins.Bin>
    {
        public double Max { get; private set; }
        public double Min { get; private set; }
        public int BinCount { get; private set; }

        protected Bin SmallOutlierBin { get; set; }
        protected Bin LargeOutlierBin { get; set; }

        public long OutlierCount { get { return SmallOutlierBin.ElementCount + LargeOutlierBin.ElementCount; } }
        public long ElementCount { get { return validCount + OutlierCount; } }
        public long ValidCount { get { return validCount; } }

        private int validCount;

        public Bins(double min, double max, int binCount)
        {
            Min = min;
            Max = max;
            BinCount = binCount;
            SmallOutlierBin = new Bin(double.MinValue, min);
            LargeOutlierBin = new Bin(max, double.MaxValue);

            var nextMin = min;
            var size = (max - min) / binCount;
            for (int i = 0; i < BinCount; i++)
            {
                Add(new Bin(nextMin, nextMin + size));
                nextMin += size;
            }
        }

        public void Add(double x)
        {
            if (x < Min)
            {
                SmallOutlierBin.Increment();
                return;
            }
            if (x > Max)
            {
                LargeOutlierBin.Increment();
                return;
            }
            var bin = BinarySearch(x);
            if (bin != null)
            {
                bin.Increment();
                Interlocked.Increment(ref validCount);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        private Bin BinarySearch(double x)
        {
            var min = 0;
            var max = BinCount;
            while (min < max)
            {
                int mid = (max - min) / 2 + min;
                var midBin = this[mid];
                if (midBin.Max < x)
                {
                    min = mid + 1;
                }
                else
                {
                    max = mid;
                }
            }
            if (max == min && this[min].Contains(x))
                return this[min];
            return null;
        }

        public class Bin
        {
            public double Max { get; protected set; }
            public double Min { get; protected set; }

            private long elementCount;
            public long ElementCount { get { return elementCount; } }

            public Bin(double min, double max)
            {
                Min = min;
                Max = max;
            }

            public void Increment()
            {
                Interlocked.Increment(ref elementCount);
            }

            public bool Contains(double x)
            {
                return Max >= x && Min <= x;
            }
        }
    }
}