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

        /// <summary>
        /// Try to find out possible 5-card combinations from a hand with 5/6/7 cards.
        /// </summary>
        /// <param name="hand"></param>
        /// <returns></returns>
        public static Hand Find(Hand hand)
        {
            if (hand.Count < 5)
                return null;

            // step 1, order by rank, find straight
            var rawStraight = hand.OrderBy(HandSortType.ByRanks).ToList();
            var previousCard = rawStraight[0];
            var possibleStraight = false;
            var possibleStraightCount = 0;
            var possibleStraightHighRank = Ranks.Any;
            for (int i = 1; i < rawStraight.Count; i++)
            {
                var card = rawStraight[i];
                if (card.Rank - previousCard.Rank == 1)
                {
                    possibleStraight = true;
                    if (possibleStraightHighRank == Ranks.Any)
                        possibleStraightHighRank = card.Rank;
                    previousCard = card;
                    possibleStraightCount++;
                }
                else
                {
                    possibleStraight = false;
                    possibleStraightCount = 0;
                }
            }
            return hand;
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
                                    hand.BestHandType = HandType.FourOfAKind;
                                    hand.HighestRank = hand[1].Rank;
                                    break;
                                case 4:
                                    hand.BestHandType = HandType.FourOfAKind;
                                    hand.HighestRank = hand[0].Rank;
                                    break;
                                case 2:
                                    hand.BestHandType = HandType.FullHouse;
                                    hand.HighestRank = hand[1].Rank;
                                    break;
                                case 3:
                                    hand.BestHandType = HandType.FullHouse;
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
                                hand.BestHandType = HandType.ThreeOfAKind;
                                hand.HighestRank = hand[2].Rank; // the mid one always have 3.
                            }
                            else
                            {
                                hand.BestHandType = HandType.TwoPair;
                                hand.HighestRank = first == 1 ? hand[2].Rank : hand[0].Rank;
                            }
                            break;
                        }
                    case 4: // 1pair
                        {
                            hand.BestHandType = HandType.OnePair;
                            hand.HighestRank = groups[hand[1].Rank] == 2 ? hand[1].Rank : hand[3].Rank;
                            break;
                        }
                    case 5: // high card (ex. straights)
                        {
                            hand.BestHandType = HandType.HighCard;
                            hand.HighestRank = hand[0].Rank;
                            break;
                        }
                }
                // all cases (non straight/flush) shall be covered.
                return;
            }

            if (isStraight && isFlush)
                hand.BestHandType = HandType.StraightFlush;
            else if (isStraight)
                hand.BestHandType = HandType.Straight;
            else
                hand.BestHandType = HandType.Flush;
        }
    }
}