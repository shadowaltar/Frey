using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Algorithms.Algos;

namespace Algorithms.Apps.TexasHoldem
{
    public class Hand : IList<Card>, IComparable<Hand>
    {
        private readonly Cards cards = new Cards();
        public Cards Cards { get { return cards; } }

        public Hand()
        {
            Groups = new Dictionary<Ranks, int>();
        }

        public Dictionary<Ranks, int> Groups { get; private set; }

        public bool IsFive { get { return cards.Count == 5; } }

        public Hand(string hand)
            : this()
        {
            var values = new List<Card>();
            var abbrs = hand.Split(' ');
            foreach (var abbr in abbrs)
            {
                var suit = abbr.Substring(0, 1);
                var rank = abbr.Replace(suit, "");
                values.Add(new Card(SuitHelper.Parse(suit), RankHelper.Parse(rank)));
            }
            values.Sort();
            AddRange(values);
            IsSorted = true;
        }

        public Hand(string hand, HandType type)
            : this(hand)
        {
            Type = type;
        }

        public Hand(params Card[] cards)
            : this()
        {
            Array.Sort(cards);
            AddRange(cards);
            IsSorted = true;
        }

        public Hand(IEnumerable<Card> cards)
            : this()
        {
            var cs = cards.ToList();
            cs.Sort();
            AddRange(cs);
            IsSorted = true;
        }

        public Hand(Cards cards)
            : this()
        {
            cards.Sort();
            AddRange(cards);
        }

        public Hand(Hand hand)
            : this()
        {
            var sorted = Sortings.QuickSort(cards);
            AddRange(sorted);
        }

        public HandType Type { get; set; }

        public Ranks SignificantRank { get; set; }

        public bool IsSorted { get; protected set; }

        public bool IsSet { get; protected set; }

        public void Set(HandType type, Ranks significantRank)
        {
            Type = type;
            SignificantRank = significantRank;
            IsSet = true;

            if (type == HandType.StraightFlush || type == HandType.Straight)
            {
                cards.SortByDescending();
                if (cards[0].Rank == Ranks.A && cards[1].Rank == Ranks.Five)
                {
                    cards.Add(cards[0]);
                    cards.RemoveAt(0);
                }
            }
            IsSorted = true;
        }

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

        public void Reverse()
        {
            cards.Reverse();
        }

        public int CompareTo(Hand two)
        {
            return CardCombinationHelper.Compare(this, two);
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

        public override string ToString()
        {
            return string.Format("{0}, {1}", Cards, Type);
        }
    }
}