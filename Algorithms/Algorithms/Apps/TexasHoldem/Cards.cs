using System;
using System.Collections.Generic;
using System.Linq;
using Algorithms.Utils;

namespace Algorithms.Apps.TexasHoldem
{
    public class Cards : List<Card>
    {
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
        public IEnumerable<Card> DistinctOrderBy(HandSortType type)
        {
            switch (type)
            {
                case HandSortType.ByRanks:
                    return this.DistinctBy(c => c.Rank).OrderBy(c => c.Rank).ThenBy(c => c.Suit);
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

        public override string ToString()
        {
            var temp = this.Take(6).ToList();
            var moreThanSix = Count > 6;
            return string.Format("{0},{1},{2},{3},{4},{5}{6}",
                temp[0], temp[1], temp[2], temp[3], temp[4], temp[5], moreThanSix ? "..." : "");
        }
    }
}