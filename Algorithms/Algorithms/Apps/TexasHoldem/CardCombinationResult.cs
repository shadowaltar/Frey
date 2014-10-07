using System.Collections.Generic;

namespace Algorithms.Apps.TexasHoldem
{
    public class CardCombinationResult
    {
        public HandType BestCombinationType { get; set; }
        public List<Card> BestCombination { get; set; }
        public CardPower PlayerCardPower { get; set; }
        public CardPower AverageOpponentCardPower { get; set; }
    }

    public class CardCombinationEvaluation
    {
        public HandType Type { get; set; }

    }
}