using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Algorithms.Collections;
using Algorithms.Randoms;

namespace Algorithms.Algos
{
    public class Games
    {
        private static readonly Random random = ThreadSafeRandom.Instance;

        public static int Dice()
        {
            return random.Next(1, 6);
        }

        /// <summary>
        /// Solve Josephus problem. Optionally can specify which person to start the elimination (default 0).
        /// </summary>
        /// <param name="mth"></param>
        /// <param name="n"></param>
        /// <param name="startAt"></param>
        /// <returns></returns>
        public static IEnumerable<int> SolveJosephus(int mth, int n, int startAt = 0)
        {
            if (mth > n)
                throw new InvalidOperationException();

            var x = startAt;
            var sources = new List<int>();
            sources.AddRange(Series.Sequence(0, n - 1));

            while (sources.Count > 0)
            {
                while (x >= n)
                    x %= n;

                yield return x;
                sources.Remove(x);

                x += mth;
            }
        }
    }
}