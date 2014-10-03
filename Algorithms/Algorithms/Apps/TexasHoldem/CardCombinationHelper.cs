using System;
using System.Collections.Generic;
using System.Linq;
using Algorithms.Algos;

namespace Algorithms.Apps.TexasHoldem
{
    public static class CardCombinationHelper
    {
        /// <summary>
        /// (Naive algo) Returns powers of all players' all card combinations.
        /// First power is for player himself.
        /// </summary>
        /// <param name="cardsOnTheTable"></param>
        /// <param name="cardRound"></param>
        /// <param name="cardsHeld"></param>
        /// <returns></returns>
        public static CardPower[] Calculate(Deck cardsOnTheTable, int cardRound, List<Card> cardsHeld)
        {
            var allCards = cardsOnTheTable.Cards.ToList();
            allCards.AddRange(cardsHeld);
            allCards = Sortings.QuickSort(allCards);

            if (allCards.Count < 5)
                return new[] { CardPower.Ok, CardPower.Ok, };
            else
                return new[] { CardPower.Ok, CardPower.Ok, };
        }

        public static bool IsPokerHandStrongerThan(Deck one, Deck two)
        {
            if (one.Cards.Count < 5 || two.Cards.Count <= 5)
                throw new InvalidOperationException();

            var a = one;
            if (one.Cards.Count > 5)

                return false;

            return false;
        }

        public static Deck FindBestPokerHand(Deck deck)
        {
            if (deck.Cards.Count < 5)
                return deck;

            deck.FindStraightFlush();
            return null;
        }

        public static void FindStraightFlush(this Deck deck)
        {
            deck.Sort();
            var spadeCount = 0;
            var heartCount = 0;
            var diamondCount = 0;
            var clubCount = 0;
            foreach (var card in deck.Cards)
            {
                switch (card.Suit)
                {
                    case Suits.Clubs:
                        clubCount++;
                        break;
                }
            }
        }

        public static bool StrongerThan(this List<Card> handOne, List<Card> handTwo)
        {
            throw new NotImplementedException();
        }
    }

    internal static class CardCombinations
    {
        public static Dictionary<int, List<Card>> Combinations = new Dictionary<int, List<Card>>
        {
            {2000, new Hand("?A ?K ?Q ?J ?10")},
            {1999, new Hand("?K ?Q ?J ?10 ?9")},
        };
    }
}