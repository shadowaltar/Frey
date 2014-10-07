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


        public static int CompareFlushRank(Hand flushOne, Hand flushTwo)
        {
            for (int i = 0; i < 5; i++)
            {
                if (flushOne[i].CompareTo(flushTwo[i]) != 0)
                    return flushOne[i].CompareTo(flushTwo[i]);
            }
            return 0;
        }

        public static void FindType(Hand hand)
        {
            bool isStraight = false;
            var isFlush = false;

            // check suits see if flush
            if (hand[0].Suit == hand[1].Suit
                && hand[0].Suit == hand[2].Suit
                && hand[0].Suit == hand[3].Suit
                && hand[0].Suit == hand[4].Suit)
            {
                isFlush = true;
                hand.HighestRank = hand[0].Rank;
            }

            // check all non-flush
            if (hand[1].Rank + 1 == hand[0].Rank
                && hand[2].Rank + 1 == hand[1].Rank
                && hand[3].Rank + 1 == hand[2].Rank
                && hand[4].Rank + 1 == hand[3].Rank)
            {
                isStraight = true;
                hand.HighestRank = hand[0].Rank;
            }
            else if (hand[0].Rank == Ranks.A
                     && hand[1].Rank == Ranks.Five
                     && hand[2].Rank == Ranks.Four
                     && hand[3].Rank == Ranks.Three
                     && hand[4].Rank == Ranks.Two)
            {
                isStraight = true;
                hand.HighestRank = Ranks.Five;
            }
            else if (!isFlush) // need to count fours/threes/pairs
            {
                var groups = hand.Groups;
                foreach (var card in hand)
                {
                    groups[card.Rank]++;
                }

                switch (groups.Count)
                {
                    case 2: // fours/fullhouse
                        {
                            var first = groups[hand[0].Rank];
                            switch (first)
                            {
                                case 1:
                                    hand.Type = HandType.FourOfAKind;
                                    hand.HighestRank = hand[1].Rank;
                                    break;
                                case 4:
                                    hand.Type = HandType.FourOfAKind;
                                    hand.HighestRank = hand[0].Rank;
                                    break;
                                case 2:
                                    hand.Type = HandType.FullHouse;
                                    hand.HighestRank = hand[1].Rank;
                                    break;
                                case 3:
                                    hand.Type = HandType.FullHouse;
                                    hand.HighestRank = hand[0].Rank;
                                    break;
                            }
                            break;
                        }
                    case 3: // threes/2pair
                        {
                            var first = groups[hand[0].Rank];
                            var second = groups[hand[1].Rank];
                            if ((first == 3 && second == 1) || (first == 1 && second == 3) || (first == 1 && second == 1))
                            {
                                hand.Type = HandType.ThreeOfAKind;
                                hand.HighestRank = hand[2].Rank; // the mid one always have 3.
                            }
                            else
                            {
                                hand.Type = HandType.TwoPair;
                                hand.HighestRank = first == 1 ? hand[2].Rank : hand[0].Rank;
                            }
                            break;
                        }
                    case 4: // 1pair
                        {
                            hand.Type = HandType.OnePair;
                            hand.HighestRank = groups[hand[1].Rank] == 2 ? hand[1].Rank : hand[3].Rank;
                            break;
                        }
                    case 5: // high card (ex. straights)
                        {
                            hand.Type = HandType.HighCard;
                            hand.HighestRank = hand[0].Rank;
                            break;
                        }
                }
                // all cases (non straight/flush) shall be covered.
                return;
            }

            if (isStraight && isFlush)
                hand.Type = HandType.StraightFlush;
            else if (isStraight)
                hand.Type = HandType.Straight;
            else
                hand.Type = HandType.Flush;
        }
    }

    /// <summary>
    /// There are 2598960 possible combinations for 5-card poker hand (52C5) with suit rule.
    /// </summary>
    internal static class FiveCardCombinations
    {
        public static Dictionary<int, Hand> Combinations = new Dictionary<int, Hand>();

        public static void InitializeStraightFlushes()
        {
            // 10 cases (ignoring suits), 2000-1991
            // from "?A ?K ?Q ?J ?10" to "?6 ?5 ?4 ?3 ?2" (rank 1992)
            for (int i = 14; i >= 6; i--)
            {
                var initStr =
                   string.Concat("?", ((Ranks)i).Name(),
                   " ?", ((Ranks)(i - 1)).Name(),
                   " ?", ((Ranks)(i - 2)).Name(),
                   " ?", ((Ranks)(i - 3)).Name(),
                   " ?", ((Ranks)(i - 4)).Name());
                Combinations[i + 1986] = new Hand(initStr, HandType.StraightFlush);
            }
            Combinations[1991] = new Hand("?5 ?4 ?3 ?2 ?A", HandType.StraightFlush);
        }

        public static void InitializeFourOfAKinds()
        {
            // 13 cases (ignoring suits), 1990-1978
            for (int i = 14; i >= 2; i--)
            {
                var temp = "?" + ((Ranks)i).Name() + " ";
                var initStr = string.Concat(temp, temp, temp, temp, "??");
                Combinations[i + 1976] = new Hand(initStr, HandType.FourOfAKind);
            }
        }

        public static void InitializeFullHouses()
        {
            // 13 cases (ignoring suits), 1977-1965
            for (int i = 14; i >= 2; i--)
            {
                var temp = "?" + ((Ranks)i).Name() + " ";
                var initStr = string.Concat(temp, temp, temp, "?? ??");
                Combinations[i + 1963] = new Hand(initStr, HandType.FullHouse);
            }
        }

        public static void InitializeStraights()
        {
            // 13 cases (ignoring suits), start at rank 1963-1951
            for (int i = 14; i >= 6; i--)
            {
                var initStr =
                   string.Concat("?", ((Ranks)i).Name(),
                   " ?", ((Ranks)(i - 1)).Name(),
                   " ?", ((Ranks)(i - 2)).Name(),
                   " ?", ((Ranks)(i - 3)).Name(),
                   " ?", ((Ranks)(i - 4)).Name());
                Combinations[i + 1949] = new Hand(initStr, HandType.StraightFlush);
            }
            Combinations[1951] = new Hand("?5 ?4 ?3 ?2 ?A", HandType.StraightFlush);
        }
    }
}