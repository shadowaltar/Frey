using System;
using System.Collections.Generic;
using System.Linq;

namespace Algorithms.Algos
{
    public static class Sortings
    {
        public static int[] Sort(int[] numbers)
        {
            return numbers.OrderBy(n => n).ToArray();
        }

        public static IEnumerable<int> Scramble(this int[] numbers)
        {
            var r = new Random();
            var results = new int[numbers.Length];
            var count = numbers.Length;
            var j = 0;
            while (true)
            {
                var i = r.Next(0, count);
                results[j] = numbers[i];
                numbers[i] = numbers[count - 1];
                count--;
                j++;
                if (count == 0)
                    break;
            }
            return results;
        }

        public static IEnumerable<double> Scramble(this double[] numbers)
        {
            var r = new Random();
            var results = new double[numbers.Length];
            var count = numbers.Length;
            var j = 0;
            while (true)
            {
                var i = r.Next(0, count);
                results[j] = numbers[i];
                numbers[i] = numbers[count - 1];
                count--;
                j++;
                if (count == 0)
                    break;
            }
            return results;
        }
    }
}