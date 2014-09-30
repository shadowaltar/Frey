using System;
using System.Diagnostics;
using Algorithms.Algos;

namespace Algorithms.Exercises
{
    public class ClosestPair
    {
        public static void Test(Func<int[], int[]> function)
        {
            var lastTime = TimeTrial(125, function);
            for (int n = 250; ; n += n)
            {
                var time = TimeTrial(n, function);
                Console.WriteLine("{0} - {1} - {2}", n, time, (double)time / lastTime);
                lastTime = time;
                if (n > 10000000)
                    break;
            }
        }

        private static long TimeTrial(int n, Func<int[], int[]> function)
        {
            var random = new Random();
            int max = 1000000;
            int[] a = new int[n];
            for (int i = 0; i < n; i++)
            {
                a[i] = random.Next(-max, max);
            }
            var timer = new Stopwatch();
            timer.Start();
            var result = function(a);
            timer.Stop();
            return timer.ElapsedMilliseconds;
        }

        /// <summary>
        /// N^2
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static int[] FindClosest(int[] values)
        {
            if (values.Length < 2)
                return null;

            var n = values.Length;
            int a = values[0];
            int b = values[1];
            var diff = (a - b).Absolute();
            for (int i = 2; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    var now = (values[i] - values[j]).Absolute();
                    if (now < diff)
                    {
                        a = values[i];
                        b = values[j];
                        diff = now;
                    }
                }
            }
            return new[] { a, b };
        }


        /// <summary>
        /// NlogN
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static int[] FindClosestFast(int[] values)
        {
            if (values.Length < 2)
                return null;

            Array.Sort(values);

            var n = values.Length;
            int a = values[0];
            int b = values[1];
            var diff = (a - b).Absolute();
            for (int i = 2; i < n - 1; i++)
            {
                var j = i + 1;
                var now = (values[i] - values[j]).Absolute();
                if (now < diff)
                {
                    a = values[i];
                    b = values[j];
                    diff = now;
                }
            }
            return new[] { a, b };
        }
    }
}