using Algorithms.Algos;
using Algorithms.Utils;
using System.Collections.Generic;
using System.Linq;

namespace Algorithms.Apps.TexasHoldem
{
    public class Deck
    {
        public Deck()
        {
            Cards = new Queue<Card>();
        }

        public Queue<Card> Cards { get; private set; }

        public void Add(Card card)
        {
            Cards.Enqueue(card);
        }

        public void Add(Deck deck)
        {
            foreach (var card in deck.Cards)
            {
                Cards.Enqueue(card);
            }
        }

        public void Merge(Deck deck)
        {
            var c = deck.Draw();
            while (c != null)
            {
                Cards.Enqueue(c);
                c = deck.Draw();
            }
        }

        public Card Draw()
        {
            if (Cards.Count > 0)
                return Cards.Dequeue();
            return null;
        }

        public bool StrongerThan(Deck another)
        {
            return false;
        }

        public static Deck NewRandom()
        {
            var deck = new Deck();
            var cards = new List<Card>();
            foreach (var rank in typeof(Ranks).Values<Ranks>())
            {
                foreach (var suit in typeof(Suits).Values<Suits>())
                {
                    cards.Add(new Card(suit, rank));
                }
            }
            foreach (var card in cards.Scramble())
            {
                deck.Cards.Enqueue(card);
            }
            return deck;
        }

        public static Deck operator +(Deck deck, Card card)
        {
            deck.Cards.Enqueue(card);
            return deck;
        }

        public static Deck operator +(Card card, Deck deck)
        {
            return deck + card;
        }

        public static void PrintCombinationProbabilities()
        {

        }

        protected bool Equals(Deck other)
        {
            return Cards.All(t => other.Cards.Contains(t));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Deck)obj);
        }

        public override int GetHashCode()
        {
            return (Cards != null ? Cards.GetHashCode() : 0);
        }

        public static bool operator ==(Deck left, Deck right)
        {
            if (left == null) return false;
            return left.Equals(right);
        }

        public static bool operator !=(Deck left, Deck right)
        {
            return left != right;
        }

        public void Sort()
        {
            for (int i = 0; i < Cards.Count; i++)
            {
                for (int j = 0; j > 0; j--)
                {

                }
            }
        }
    }
}