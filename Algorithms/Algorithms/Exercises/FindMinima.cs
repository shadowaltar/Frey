using System;
using System.Diagnostics;
using Algorithms.Algos;

namespace Algorithms.Exercises
{
    public class FindMinima
    {
        public static void Test1DArray(Func<int[], int> function)
        {
            var lastTime = 0L;
            for (int n = 125; ; n += n)
            {
                var inputs = GetRandoms(n);
                var timer = new Stopwatch();
                timer.Start();

                function(inputs);

                timer.Stop();
                var time = timer.ElapsedMilliseconds;


                Console.WriteLine("{0} - {1} - {2}", n, time, (double)time / lastTime);

                lastTime = time;

                if (n > 10000000)
                    break;
            }
        }

        private static int[] GetRandoms(int count)
        {
            var values = Series.Sequence(count);
            return values.Scramble();
        }

        /// <summary>
        /// Return the index of a local minimum in a 2D array of integers. Assume it is nxn.
        /// This algo goes to 'left side' first.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static int[] Find2DArrayOneLocalMinimum(int[][] values)
        {
            if (values.Length < 1)
                throw new InvalidOperationException();
            if (values.Length == 1)
                return new[] { 0 };
            if (values.Length == 2)
            {
                if (values[0][0] < values[0][1] && values[0][0] < values[1][0])
                    return new[] { 0, 0 };
                if (values[1][0] < values[0][0] && values[1][0] < values[1][1])
                    return new[] { 1, 0 };
                if (values[0][1] < values[0][0] && values[0][1] < values[1][1])
                    return new[] { 0, 1 };
                if (values[1][1] < values[1][0] && values[1][1] < values[0][1])
                    return new[] { 1, 1 };
            }

            var n = values.Length;
            int i = n / 2;
            int j = n / 2;

            while (true)
            {
                // edge cases
                if (i == n - 1 || i == 0)
                {
                    j = Find1DArrayOneLocalMinimum(values[i]);
                    return new[] { i, j };
                }
                if (j == n - 1 || j == 0)
                {
                    i = Find1DArrayOneLocalMinimum(values.GetColumn(j));
                    return new[] { i, j };
                }

                // middle cases
                if (values[i - 1][j] < values[i][j])
                {
                    i--;
                }
                else if (values[i + 1][j] < values[i][j])
                {
                    i++;
                }
                else if (values[i - 1][j] > values[i][j] && values[i + 1][j] > values[i][j])
                {
                    if (values[i][j - 1] < values[i][j])
                    {
                        j--;
                    }
                    else if (values[i][j + 1] < values[i][j])
                    {
                        j++;
                    }
                    else if (values[i][j - 1] > values[i][j] && values[i][j + 1] > values[i][j])
                    {
                        return new[] { i, j };
                    }
                }
            }
        }

        /// <summary>
        /// Return the index of a local minimum in a 1D array of integers.
        /// This algo goes to 'left side' first.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static int Find1DArrayOneLocalMinimum(int[] values)
        {
            if (values.Length < 1)
                throw new InvalidOperationException();
            if (values.Length == 1)
                return 0;
            if (values.Length == 2)
                return values[0] > values[1] ? 1 : 0;

            var n = values.Length;
            int index = n / 2;
            while (true)
            {
                if (index == 0 || index == n - 1)
                    return index;

                if (values[index - 1] < values[index])
                {
                    index--;
                }
                else if (values[index + 1] < values[index])
                {
                    index++;
                }
                else if (values[index - 1] > values[index] && values[index + 1] > values[index])
                {
                    Console.WriteLine(index);
                    return index;
                }
            }
        }
    }
}