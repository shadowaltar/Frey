using System;
using System.Diagnostics;
using Algorithms.Algos;

namespace Algorithms.Exercises
{
    public class FarthestPair
    {
        public static void Test(Func<int[], int[]> function)
        {
            var lastTime = 0l;
            for (int n = 125; ; n += n)
            {
                var inputs = GetRandoms(n);
                var timer = new Stopwatch();
                timer.Start();

                function(inputs);
                
                timer.Stop();
                var time = timer.ElapsedMilliseconds;

                if (lastTime != 0)
                    Console.WriteLine("{0} - {1} - {2}", n, time, (double)time / lastTime);
                
                lastTime = time;
                
                if (n > 10000000)
                    break;
            }
        }

        private static int[] GetRandoms(int count, int valueMax = 1000000)
        {
            var random = new Random();
            int[] a = new int[count];
            for (int i = 0; i < count; i++)
            {
                a[i] = random.Next(-valueMax, valueMax);
            }
            return a;
        }

        /// <summary>
        /// N
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static int[] FindFarthestFast(int[] values)
        {
            if (values.Length < 2)
                return null;

            Array.Sort(values);

            return new[] { values[0], values[values.Length - 1] };
        }
    }
}