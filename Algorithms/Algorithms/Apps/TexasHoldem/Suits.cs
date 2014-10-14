using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Algorithms.Apps.TexasHoldem
{
    public enum Suits
    {
        Spades = 4,
        Hearts = 3,
        Diamonds = 2,
        Clubs = 1,
        Any = 0,
    }

    public static class SuitHelper
    {
        public static string Name(this Suits suit)
        {
            switch (suit)
            {
                case Suits.Clubs:
                    return "C";
                    return "\u2663";
                case Suits.Spades:
                    return "S";
                    return "\u2660";
                case Suits.Hearts:
                    return "H";
                    return "\u2665";
                case Suits.Diamonds:
                    return "D";
                    return "\u2666";
                case Suits.Any:
                    return "?";
            }
            throw new InvalidOperationException();
        }

        public static Suits Parse(this string str)
        {
            switch (str.ToUpper())
            {
                case "S":
                    return Suits.Spades;
                case "H":
                    return Suits.Hearts;
                case "D":
                    return Suits.Diamonds;
                case "C":
                    return Suits.Clubs;
                case "?":
                    return Suits.Any;
            }
            throw new InvalidOperationException();
        }

        public static IEnumerable<Suits> Values()
        {
            for (int i = 4; i > 0; i--)
            {
                yield return (Suits)i;
            }
        }
    }
}