namespace Algorithms.Apps.TexasHoldem
{
    public enum HandType
    {
        StraightFlush,
        FourOfAKind,
        FullHouse,
        Flush,
        Straight,
        ThreeOfAKind,
        TwoPair,
        OnePair,
        HighCard,

        Unknown,
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