using System;
using Algorithms.Algos;

namespace Algorithms.Utils
{
    public class Accumulator
    {
        private double sum;
        private double squaredSum;
        private double mean;
        private int n;

        public void Add(double value)
        {
            n++;
            sum += value;
            squaredSum += ((n - 1.0) / n * (value - mean).Power(2));
            mean += ((value - mean) / n);
        }

        public double Sum()
        {
            return sum;
        }

        public double Mean()
        {
            return mean;
        }

        public double PopulationVariance()
        {
            return squaredSum / n;
        }

        public double SampleVariance()
        {
            return squaredSum / (n - 1);
        }

        public double PopulationStandardDeviation()
        {
            return Math.Sqrt(PopulationVariance());
        }

        public double SampleStandardDeviation()
        {
            return Math.Sqrt(SampleVariance());
        }
    }
}