using System;
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
    }
}