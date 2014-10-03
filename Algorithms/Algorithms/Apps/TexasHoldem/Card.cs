using System;
using System.Collections.Generic;
using Algorithms.Algos;

namespace Algorithms.Apps.TexasHoldem
{
    public class Hand : List<Card>
    {
        public Hand(string hand)
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

        public Hand(List<Card> cards)
        {
            var sorted = Sortings.QuickSort(cards);
            AddRange(sorted);
        }
    }

    public class Card : IComparable<Card>
    {
        public Suits Suit { get; set; }
        public Ranks Rank { get; set; }

        public Card(Suits suit, Ranks rank)
        {
            Suit = suit;
            Rank = rank;
        }

        public int CompareTo(Card other)
        {
            return Rank > other.Rank
                ? 1
                : Rank == other.Rank
                ? Suit.CompareTo(other.Suit)
                : -1;
        }

        public override string ToString()
        {
            return Suit.Name() + Rank.Name();
        }
    }
}