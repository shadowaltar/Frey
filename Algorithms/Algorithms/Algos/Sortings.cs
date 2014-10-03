using System;
using System.Collections.Generic;

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
        public static List<T> QuickSort<T>(List<T> input)
            where T : IComparable<T>
        {
            if (input.Count == 1)
                return input;

            input = input.Scramble();
            InternalQuickSort(input, 0, input.Count - 1);
            return input;
        }

        private static void InternalQuickSort<T>(List<T> inputs, int low, int high)
            where T : IComparable<T>
        {
            if (high <= low)
                return;
            int j = QuickSortPartition(inputs, low, high);
            InternalQuickSort(inputs, low, j - 1);
            InternalQuickSort(inputs, j + 1, high);
        }

        private static int QuickSortPartition<T>(List<T> inputs, int low, int high)
            where T : IComparable<T>
        {
            int i = low, j = high + 1;
            var v = inputs[low];
            while (true)
            {
                while (LessThan(inputs[++i], v)) if (i == high) break;
                while (LessThan(v, inputs[--j])) if (j == low) break;
                if (i >= j) break;
                Exchange(inputs, i, j);
            }
            Exchange(inputs, low, j);
            return j;
        }

        private static void Exchange<T>(List<T> inputs, int i, int j) where T : IComparable<T>
        {
            var t = inputs[i];
            inputs[i] = inputs[j];
            inputs[j] = t;
        }

        private static bool LessThan<T>(T a, T b) where T : IComparable<T>
        {
            return a.CompareTo(b) < 0;
        }

        /// <summary>
        /// Scramble the sequence of given set of numbers.
        /// Pick random number i from 0 to n-1, save numbers[i] to results[0], put numbers.last() into numbers[i];
        /// pick random number i from 0 to n-2, save numbers[i] to results[1], put numbers.last() into numbers[i];
        /// ...
        /// </summary>
        /// <param name="numbers"></param>
        /// <returns></returns>
        public static T[] Scramble<T>(this T[] numbers)
        {
            var r = new Random();
            var results = new T[numbers.Length];
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

        public static List<T> Scramble<T>(this List<T> numbers)
        {
            var r = new Random();
            var results = new List<T>();
            var count = numbers.Count;
            while (true)
            {
                var i = r.Next(0, count);
                results.Add(numbers[i]);
                numbers[i] = numbers[count - 1];
                count--;
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