using System;
using System.Collections.Generic;
using System.Linq;
using Algorithms.Utils;

namespace Algorithms.Apps.TexasHoldem
{
    public class Cards : List<Card>
    {
        public Cards()
        { }

        public Cards(IEnumerable<Card> cards)
            : base(cards)
        { }

        /// <summary>
        /// Sort the cards.
        /// </summary>
        public IEnumerable<Card> OrderBy(HandSortType type)
        {
            switch (type)
            {
                case HandSortType.ByRanks:
                    return this.OrderBy(c => c.Rank).ThenBy(c => c.Suit);
                case HandSortType.ByRanksReversed:
                    return this.OrderByDescending(c => c.Rank).ThenByDescending(c => c.Suit);
                case HandSortType.BySuits:
                    return this.OrderBy(c => c.Suit).ThenBy(c => c.Rank);
                case HandSortType.BySuitsReversed:
                    return this.OrderByDescending(c => c.Suit).ThenByDescending(c => c.Rank);
                default:
                    throw new NotImplementedException();
            }
        }

        public void SortByDescending()
        {
            Sort((c1, c2) => -c1.CompareTo(c2));
        }

        public IEnumerable<Card> DistinctOrderBy(HandSortType type)
        {
            switch (type)
            {
                case HandSortType.ByRanks:
                    //          return this.DistinctBy(c => c.Rank).OrderBy(c => c.Rank).ThenBy(c => c.Suit);
                    return SortByRanks();
                case HandSortType.ByRanksReversed:
                    return this.DistinctBy(c => c.Rank).OrderByDescending(c => c.Rank).ThenByDescending(c => c.Suit);
                case HandSortType.BySuits:
                    return this.DistinctBy(c => c.Suit).OrderBy(c => c.Suit).ThenBy(c => c.Rank);
                case HandSortType.BySuitsReversed:
                    return this.DistinctBy(c => c.Suit).OrderByDescending(c => c.Suit).ThenByDescending(c => c.Rank);
                default:
                    throw new NotImplementedException();
            }
        }

        private IEnumerable<Card> SortByRanks()
        {
            var cards = new List<Card>(this);
            cards.Sort();
            Card previous = null;
            foreach (var card in cards)
            {
                if (previous != card)
                {
                    yield return card;
                }
                previous = card;
            }
        }

        public static IEnumerable<Card> GetCards(Ranks rank)
        {
            var cards = new Cards();
            foreach (var suit in SuitHelper.Values())
            {
                cards.Add(new Card(suit, rank));
            }
            return cards;
        }

        public override string ToString()
        {
            var temp = this.Take(7).ToList();
            var more = Count > 7;
            var result = "";
            foreach (var card in temp)
            {
                result += card + ",";
            }
            result = result.Trim(',');
            return result + (more ? "..." : "");
        }
    }
}