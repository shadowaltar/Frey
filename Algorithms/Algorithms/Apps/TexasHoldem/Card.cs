using System;

namespace Algorithms.Apps.TexasHoldem
{
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