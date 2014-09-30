using System;
using System.Diagnostics;

namespace Algorithms.Exercises
{
    public class DoublingTest
    {
        public static void Test(Func<int[], int> function)
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

        private static long TimeTrial(int n, Func<int[], int> function)
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
            function(a);
            timer.Stop();
            return timer.ElapsedMilliseconds;
        }

        public static int ThreeSumCount(int[] a)
        {
            int n = a.Length;
            int cnt = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    for (int k = j + 1; k < n; k++)
                    {
                        if (a[i] + a[j] + a[k] == 0)
                            cnt++;
                    }
                }
            }
            return cnt;
        }

        public static int TwoSumCount(int[] a)
        {
            int n = a.Length;
            int cnt = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    if (a[i] + a[j] == 0)
                        cnt++;
                }
            }
            return cnt;
        }

        /// <summary>
        /// O(NlogN)
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static int TwoSumCountFast(int[] a)
        {
            Array.Sort(a); // mergesort, o(nlogn)
            int n = a.Length;
            int cnt = 0;
            for (int i = 0; i < n; i++)
            {
                if (Array.BinarySearch(a, -a[i]) > i) // binarysearch, o(logn)
                    cnt++;
            }
            return cnt;
        }

        /// <summary>
        /// O(N^2logN)
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static int ThreeSumCountFast(int[] a)
        {
            Array.Sort(a); // mergesort, o(nlogn)
            int n = a.Length;
            int cnt = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; i < n; i++)
                {
                    if (Array.BinarySearch(a, -a[i] - a[j]) > i) // binarysearch, o(logn)
                        cnt++;
                }
            }
            return cnt;
        }
    }
}