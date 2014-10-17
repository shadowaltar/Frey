using System;
using System.Collections.Generic;
using Algorithms.Utils;

namespace Algorithms.Algos
{
    public static class Sortings
    {
        /// <summary>
        /// Worst N^2, best N^2, average N^2; in-place.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static void SelectionSort(int[] input)
        {
            var n = input.Length;
            int min;
            for (int i = 0; i < n; i++)
            {
                min = i;
                for (int j = i + 1; j < n; j++)
                {
                    if (input[j] < input[min])
                    {
                        min = j;
                    }
                }
                Exchange(input, i, min);
            }
        }

        /// <summary>
        /// Worst N^2, best N, average N^2; stable; adaptive; in-place.
        /// </summary>
        /// <param name="input"></param>
        public static void InsertionSort(int[] input)
        {
            var n = input.Length;
            for (int i = 1; i < n; i++)
            {
                for (int j = i; j > 0 && input[j] < input[j - 1]; j--)
                {
                    Exchange(input, j, j - 1);
                }
            }
        }

        /// <summary>
        /// Worst N^2, best NlogN; average ?; unstable; adaptive.
        /// </summary>
        /// <param name="input"></param>
        public static void ShellSort(int[] input)
        {
            int n = input.Length;
            int h = 1;
            while (h < n / 3)
                h = 3 * h + 1;
            while (h >= 1)
            {
                for (int i = h; i < n; i++)
                {
                    for (int j = i; j >= h && input[j] < input[j - h]; j -= h)
                    {
                        Exchange(input, j, j - h);
                    }
                }
                h /= 3;
            }
        }

        /// <summary>
        /// O(NlogN) sorting algorithm.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static void MergeSort(int[] input)
        {
            throw new NotImplementedException();
        }


        private static void InternalMerge(int[] input, int low, int mid, int high)
        {
            var cache = new int[input.Length];
            int i = low, j = mid + 1;
            for (int k = low; k <= high; k++)
            {
                cache[k] = input[k];
            }

            for (int k = low; k <= high; k++)
            {
                if (i > mid) input[k] = cache[j++];
                else if (j > high) input[k] = cache[i++];
                else if (cache[j] < cache[i]) input[k] = cache[j++];
                else input[k] = cache[i++];
            }
        }

        public static int[] BubbleSort(int[] input)
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

        private static void Exchange(int[] array, int a, int b)
        {
            var t = array[b];
            array[b] = array[a];
            array[a] = t;
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
            var results = new List<T>();
            var count = numbers.Count;
            while (true)
            {
                var i = StaticRandom.Instance.Next(1, count);
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