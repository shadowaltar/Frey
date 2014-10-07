using System;
using System.Collections.Generic;
using System.Linq;

namespace Algorithms.Algos
{
    public static class Series
    {
        public static int[] Sequence(int length)
        {
            var result = new int[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = i;
            }
            return result;
        }

        public static IEnumerable<int> Sequence(int min, int max)
        {
            for (int i = min; i <= max; i++)
            {
                yield return i;
            }
        }

        public static IEnumerable<int> RandomSequence(int min, int max)
        {
            var sequence = Sequence(min, max);
            return sequence.ToArray().Scramble();
        }

        public static IEnumerable<int> RandomArray(int count, int min, int max)
        {
            var r = new Random((int)DateTime.Now.Ticks);
            for (int i = 0; i < count; i++)
            {
                yield return r.Next(min, max);
            }
        }

        public static T[] GetColumn<T>(this T[][] array, int columnIndex)
        {
            var n = array.Length;
            var results = new T[n];
            for (int i = 0; i < n; i++)
            {
                results[i] = array[i][columnIndex];
            }
            return results;
        }

        public static long[] GetFibonacciNumbers(int count)
        {
            return Fibonacci.Get(count);
        }

        public static long GetFibonacciNumberAt(int index)
        {
            return Fibonacci.FFast(index);
        }

        internal class Fibonacci
        {
            internal static long FSlow(int index)
            {
                if (index > 100)
                    return Get(index)[index - 1];
                if (index == 0) return 0;
                if (index == 1) return 1;
                return FSlow(index - 1) + FSlow(index - 2);
            }
            internal static long FFast(int index)
            {
                if (index < 5)
                    return FSlow(index);

                var ph = (1 + Arithmetics.SquareRoot(5)) / 2.0;
                return (ph.Power(index) / Arithmetics.SquareRoot(5)).Rounding();
            }
            internal static long[] Get(int count)
            {
                if (count <= 0) return null;
                var result = new long[count];
                result[0] = 1;
                if (count > 1)
                {
                    result[1] = 1;
                    for (int i = 2; i < count; i++)
                    {
                        result[i] = result[i - 1] + result[i - 2];
                        if (result[i] < 0)
                            throw new OverflowException("Too big for long");
                    }
                }
                return result;
            }
        }
    }
}