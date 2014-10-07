using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Algorithms.Algos;

namespace Algorithms.Apps.TexasHoldem
{
    public class Hand : IList<Card>, IComparable<Hand>
    {
        private List<Card> cards = new List<Card>();

        public Hand()
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
            IsSorted = true;
            AddRange(cards);
        }

        public Hand(string hand, HandType bestHandType)
            : this(hand)
        {
            BestHandType = bestHandType;
        }

        public Hand(List<Card> cards)
            : this()
        {
            var sorted = Sortings.QuickSort(cards);
            IsSorted = true;
            AddRange(sorted);
        }

        public Hand(Hand hand)
            : this()
        {
            var sorted = Sortings.QuickSort(cards);
            AddRange(sorted);
        }

        public HandType BestHandType { get; set; }

        public Ranks HighestRank { get; set; }

        public bool IsSorted { get; protected set; }

        /// <summary>
        /// Sort the cards.
        /// </summary>
        public IEnumerable<Card> OrderBy(HandSortType type)
        {
            switch (type)
            {
                case HandSortType.ByRanks:
                    return cards.OrderBy(c => c.Rank).ThenBy(c => c.Suit);
                case HandSortType.ByRanksReversed:
                    return cards.OrderByDescending(c => c.Rank).ThenByDescending(c => c.Suit);
                case HandSortType.BySuits:
                    return cards.OrderBy(c => c.Suit).ThenBy(c => c.Rank);
                case HandSortType.BySuitsReversed:
                    return cards.OrderByDescending(c => c.Suit).ThenByDescending(c => c.Rank);
                default:
                    throw new NotImplementedException();
            }
        }

        public int CompareTo(Hand two)
        {
            if (Count < 5 || two.Count < 5)
                throw new InvalidOperationException();

            if (BestHandType == HandType.Unknown)
                CardCombinationHelper.FindType(this);
            if (two.BestHandType == HandType.Unknown)
                CardCombinationHelper.FindType(two);

            if (BestHandType > two.BestHandType)
                return 1;
            if (BestHandType < two.BestHandType)
                return -1;

            // same type:
            switch (BestHandType)
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
                    break;
            }

            return 0;
        }

        public IEnumerator<Card> GetEnumerator()
        {
            return cards.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(Card item)
        {
            cards.Add(item); IsSorted = false;
        }

        public void Clear()
        {
            cards.Clear(); IsSorted = false;
        }

        public bool Contains(Card item)
        {
            return cards.Contains(item);
        }

        public void CopyTo(Card[] array, int arrayIndex)
        {
            cards.CopyTo(array, arrayIndex);
        }

        public bool Remove(Card item)
        {
            var result = cards.Remove(item);
            if (result)
                IsSorted = false;
            return result;
        }

        public int Count { get { return cards.Count; } }
        public bool IsReadOnly { get { return ((ICollection<Card>)cards).IsReadOnly; } }

        public int IndexOf(Card item)
        {
            return cards.IndexOf(item);
        }

        public void Insert(int index, Card item)
        {
            cards.Insert(index, item);
            IsSorted = false;
        }

        public void RemoveAt(int index)
        {
            cards.RemoveAt(index);
            IsSorted = false;
        }

        public void AddRange(IEnumerable<Card> values)
        {
            foreach (var value in values)
            {
                Add(value);
            }
            IsSorted = false;
        }

        public Card this[int index]
        {
            get { return cards[index]; }
            set
            {
                cards[index] = value;
                IsSorted = false;
            }
        }
    }
}