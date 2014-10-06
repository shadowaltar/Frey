using System;
using System.Collections.Generic;
using System.Linq;
using Algorithms.Algos;

namespace Algorithms.Apps.TexasHoldem
{
    public class Hand : List<Card>, IComparable<Hand>
    {
        private Hand()
        {
            Groups = new Dictionary<Ranks, int>();
        }

        public Dictionary<Ranks, int> Groups { get; private set; }

        public Hand(string hand)
            : this()
        {
            var cards = new List<Card>();
            var abbrs = hand.Split(' ');
            foreach (var abbr in abbrs)
            {
                var suit = abbr.Substring(0, 1);
                var rank = abbr.Replace(suit, "");
                cards.Add(new Card(SuitHelper.Parse(suit), RankHelper.Parse(rank)));
            }
            cards = Sortings.QuickSort(cards);
            AddRange(cards);
        }

        public Hand(string hand, HandType type)
            : this(hand)
        {
            Type = type;
        }

        public Hand(List<Card> cards)
            : this()
        {
            var sorted = Sortings.QuickSort(cards);
            AddRange(sorted);
        }

        public HandType Type { get; set; }

        public Ranks HighestRank { get; set; }

        public int CompareTo(Hand two)
        {
            if (Count < 5 || two.Count < 5)
                throw new InvalidOperationException();

            if (Type == HandType.Unknown)
                CardCombinationHelper.FindType(this);
            if (two.Type == HandType.Unknown)
                CardCombinationHelper.FindType(two);

            if (Type > two.Type)
                return 1;
            if (Type < two.Type)
                return -1;

            // same type:
            switch (Type)
            {
                case HandType.StraightFlush:
                case HandType.FourOfAKind:
                case HandType.FullHouse:
                case HandType.Straight:
                case HandType.ThreeOfAKind:
                    return HighestRank.CompareTo(two.HighestRank);
                case HandType.Flush:
                    return CardCombinationHelper.CompareFlushRank(this, two);
                case HandType.TwoPair:
            }
        }
    }
}