using System;
using System.Collections.Generic;
using System.Linq;

namespace Algorithms.Apps.TexasHoldem
{
    public enum Ranks
    {
        Any = 0,

        A = 14,
        K = 13,
        Q = 12,
        J = 11,
        Ten = 10,
        Nine = 9,
        Eight = 8,
        Seven = 7,
        Six = 6,
        Five = 5,
        Four = 4,
        Three = 3,
        Two = 2,
    }

    public static class RankHelper
    {
        public static string Name(this Ranks rank)
        {
            if ((int)rank < 11)
                return ((int)rank).ToString();
            return rank.ToString();
        }

        public static Ranks Parse(this string str)
        {
            switch (str.ToUpper())
            {
                case "A":
                    return Ranks.A;
                case "K":
                    return Ranks.K;
                case "Q":
                    return Ranks.Q;
                case "J":
                    return Ranks.J;
                case "X":
                case "10":
                    return Ranks.Ten;
                case "?":
                    return Ranks.Any;
                default:
                    int val;
                    if (int.TryParse(str, out val))
                        return (Ranks)val;
                    break;
            }
            throw new InvalidOperationException();
        }

        public static IEnumerable<string> StringValues()
        {
            for (int i = 14; i >= 2; i--)
            {
                yield return ((Ranks)i).ToString();
            }
        }
    }
}