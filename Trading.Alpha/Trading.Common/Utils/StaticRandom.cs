using System;
using System.Threading;

namespace Trading.Common.Utils
{
    public static class StaticRandom
    {
        private static int seed;

        private static readonly ThreadLocal<Random> threadLocal = new ThreadLocal<Random>
            (() => new Random(Interlocked.Increment(ref seed)));

        static StaticRandom()
        {
            seed = Environment.TickCount;
        }

        public static Random Instance { get { return threadLocal.Value; } }

        public static int ThrowDice(int faces = 6)
        {
            return Instance.Next(0, faces + 1);
        }
    }
}