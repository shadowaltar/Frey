using System;
using System.Collections.Generic;
using System.Linq;

namespace Algorithms.Algos
{
    public static class Filterings
    {
        public static T Max<T>(this IEnumerable<T> numbers) where T : IComparable
        {
            T max = default(T);
            foreach (var comparable in numbers)
            {
                max = comparable.CompareTo(max) > 0 ? comparable : max;
            }
            return max;
        }

        public static T Min<T>(this IEnumerable<T> numbers) where T : IComparable
        {
            T min = default(T);
            foreach (var comparable in numbers)
            {
                min = comparable.CompareTo(min) < 0 ? comparable : min;
            }
            return min;
        }

        public static T[] MinMax<T>(this IEnumerable<T> numbers) where T : IComparable
        {
            T min = default(T);
            T max = default(T);
            foreach (var comparable in numbers)
            {
                min = comparable.CompareTo(min) < 0 ? comparable : min;
                max = comparable.CompareTo(max) > 0 ? comparable : max;
            }
            return new[] { min, max };
        }

        public static double Median(this IEnumerable<double> numbers)
        {
            var sorted = numbers.OrderBy(n => n).ToArray();
            if (sorted.Length % 2 == 1)
                return sorted[(sorted.Length - 1) / 2];
            return (sorted[sorted.Length / 2] + sorted[(sorted.Length - 1) / 2]) / 2.0;
        }

        public static double Median(this IEnumerable<int> numbers)
        {
            var sorted = numbers.OrderBy(n => n).ToArray();
            if (sorted.Length % 2 == 1)
                return sorted[(sorted.Length - 1) / 2];
            return (sorted[sorted.Length / 2] + sorted[(sorted.Length - 1) / 2]) / 2.0;
        }

        public static double Sum(this IEnumerable<double> numbers)
        {
            double result = 0;
            foreach (var number in numbers)
            {
                result += number;
            }
            return result;
        }

        public static double SquaredSum(this IEnumerable<double> numbers)
        {
            double result = 0;
            foreach (var number in numbers)
            {
                result += number * number;
            }
            return result;
        }

        public static double Average(this IEnumerable<double> numbers)
        {
            double result = 0;
            int count = 0;
            foreach (var number in numbers)
            {
                result += number;
                count++;
            }
            return result / count;
        }

        public static double PercentageMoreThanAverage(this IEnumerable<double> numbers)
        {
            var n = numbers.ToArray();
            var avg = n.Average();
            var largerCount = 0;
            foreach (var number in n)
            {
                if (number > avg)
                    largerCount++;
            }
            return largerCount / (double)n.Length;
        }
    }
}