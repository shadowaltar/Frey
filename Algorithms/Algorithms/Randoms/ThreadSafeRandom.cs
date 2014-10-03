using Algorithms.Algos;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Algorithms.Randoms
{
    public static class ThreadSafeRandom
    {
        private static readonly Random globalRandom = new Random();
        private static readonly object globalLock = new object();
        private static readonly ThreadLocal<Random> threadRandom = new ThreadLocal<Random>(NewRandom);

        public static Random Instance
        {
            get { return threadRandom.Value; }
        }

        public static Random NewRandom()
        {
            Random result;
            lock (globalLock)
            {
                result = new Random(globalRandom.Next());
            }
            return result;
        }

        public static int Next()
        {
            return Instance.Next();
        }

        public static int Next(int maxValue)
        {
            return Instance.Next(maxValue);
        }

        public static int Next(int minValue, int maxValue)
        {
            return Instance.Next(minValue, maxValue);
        }

        public static double NextDouble()
        {
            return Instance.NextDouble();
        }

        public static void NextBytes(byte[] buffer)
        {
            Instance.NextBytes(buffer);
        }

        public static int Next(int minValue, int maxValue, int specialValue, double specialValueProbability)
        {
            if (specialValue < minValue || specialValue > maxValue || specialValueProbability < 0.0 ||
                specialValueProbability > 1.0)
            {
                throw new ArgumentException();
            }
            IEnumerable<int> items = Series.Sequence(minValue, maxValue);
            var frequencyMap = new Dictionary<int, double>();
            int commonItemCount = maxValue - minValue + 1 - 1;
            foreach (int item in items)
            {
                double freq;
                if (item != specialValue)
                {
                    freq = (1.0 - specialValueProbability) / commonItemCount;
                }
                else
                {
                    freq = specialValueProbability;
                }
                frequencyMap[item] = ((item == 0) ? 0.0 : frequencyMap[item - 1]) + freq;
            }
            double d = NextDouble();
            int result;
            foreach (var pair in frequencyMap)
            {
                if (d <= pair.Value)
                {
                    result = pair.Key;
                    return result;
                }
            }
            result = maxValue;
            return result;
        }
    }
}