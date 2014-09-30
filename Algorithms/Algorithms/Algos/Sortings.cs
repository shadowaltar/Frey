using System;
using System.Linq;

namespace Algorithms.Algos
{
    public static class Sortings
    {
        public static int[] BubbleSort(int[] input)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// O(NlogN) sorting algorithm.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int[] MergeSort(int[] input)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// O(NlogN) sorting algorithm.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int[] QuickSort(int[] input)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Scramble the sequence of given set of numbers.
        /// Pick random number i from 0 to n-1, save numbers[i] to results[0], put numbers.last() into numbers[i];
        /// pick random number i from 0 to n-2, save numbers[i] to results[1], put numbers.last() into numbers[i];
        /// ...
        /// </summary>
        /// <param name="numbers"></param>
        /// <returns></returns>
        public static int[] Scramble(this int[] numbers)
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

        public static double[] Scramble(this double[] numbers)
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

        public static void Shuffle(this int[] numbers)
        {
            int n = numbers.Length;
            for (int i = 0; i < n; i++)
            {
                int r = i + Randoms.ThreadSafeRandom.Next(0, n - i);
                var t = numbers[i];
                numbers[i] = numbers[r];
                numbers[r] = t;
            }
        }
    }
}