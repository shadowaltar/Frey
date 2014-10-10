using System;

namespace Algorithms.Apps.TexasHoldem
{
    [Flags]
    public enum HandType
    {
        Unknown = 0,

        StraightFlush = 256,
        FourOfAKind = 128,
        FullHouse = 64,
        Flush = 32,
        Straight = 16,
        ThreeOfAKind = 8,
        TwoPair = 4,
        OnePair = 2,
        HighCard = 1,
    }

    public enum HandSortType
    {
        ByRanks, // like 'A A A Q J 10 9 8 6 6 3 3 2'
        ByRanksReversed, // like '2 3 3 6 6 8 9 10 J Q A A A'
        BySuits, // try to order by flush first, then by rank; hands on the left
        BySuitsReversed, // try to find flushs first, then by rank; hands on the right
        HandsOrdinary, // like 'A A A 3 3 Q J 10 9 8 6 6 2'
        HandsOrdinaryReversed, // like '2 6 6 8 9 10 J Q A A A 3 3'
    }
}