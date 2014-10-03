using System.Collections.Generic;

namespace Algorithms.Apps.TexasHoldem
{
    public class CardCombinationResult
    {
        public CardCombinationType BestCombinationType { get; set; }
        public List<Card> BestCombination { get; set; }
        public CardPower PlayerCardPower { get; set; }
        public CardPower AverageOpponentCardPower { get; set; }
    }

    public class CardCombinationEvaluation
    {
        public CardCombinationType Type { get; set; }

    }

    public enum CardCombinationType
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
    }
}