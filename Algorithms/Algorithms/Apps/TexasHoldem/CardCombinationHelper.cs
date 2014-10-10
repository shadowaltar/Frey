using System.Collections.Generic;
using System.Linq;
using Algorithms.Algos;
using Algorithms.Utils;

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
        public static CardPower[] Calculate(List<Card> cardsOnTheTable, int cardRound, List<Card> cardsHeld)
        {
            var allCards = new List<Card>(cardsOnTheTable);
            allCards.AddRange(cardsHeld);
            allCards = Sortings.QuickSort(allCards);

            if (allCards.Count < 5)
                return new[] { CardPower.Ok, CardPower.Ok, };
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
        /// <param name="cards"></param>
        /// <returns></returns>
        public static Hand Find(Cards cards)
        {
            if (cards.Count < 5)
                return null;

            // step 1, order by suit, find flush
            var hand = FindFirstFlush(cards);
            // step 2, see if its a straight flush
            var isStraightFlush = hand != null && TestStraight(hand.ToCards());
            if (isStraightFlush)
            {
                hand.BestHandType = HandType.StraightFlush;
                return hand;
            }
            // step 3, find 4-of-a-kind
            hand = FindFourOfAKind(cards);
            if (hand != null)
            {
                hand.BestHandType = HandType.FourOfAKind;
                return hand;
            }
            // step 4, order by rank, find straight

            var sortedByRank = 
            var straight = FindStraight(sortedByRank);
            var previousCard = sortedByRank[0];
            var possibleStraight = false;
            var possibleStraightCount = 0;
            var possibleStraightHighRank = Ranks.Any;
            for (int i = 1; i < sortedByRank.Count; i++)
            {
                var card = sortedByRank[i];
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

        private static Hand FindFourOfAKind(Cards cards)
        {
            var groupOfCards = cards.GroupBy(c => c.Rank)
                .Where(g => g.Count() == 4).OrderByDescending(g => g.Key).ToList();
            if (groupOfCards.Count == 0)
                return null;
            return new Hand(groupOfCards[0]);
        }

        /// <summary>
        /// Returns the first flush, with maximum ranks.
        /// </summary>
        /// <param name="cards"></param>
        /// <returns></returns>
        private static Hand FindFirstFlush(Cards cards)
        {
            var n = cards.Count;
            if (n < 5)
                return null;
            foreach (var suit in typeof(Suits).Values(Suits.Any))
            {
                var suitCards = cards.Where(c => c.Suit == suit).ToArray();
                if (suitCards.Length == 5)
                {
                    return new Hand(suitCards.ToArray());
                }
                if (suitCards.Length > 5)
                {
                    return new Hand(suitCards.OrderByDescending(c => c.Rank).Take(5));
                }
            }
            return null;
        }

        private static Hand FindStraight(Cards cards)
        {
            var sorted = cards.DistinctOrderBy(HandSortType.ByRanks).ToList();
            // assuming sorted distinct by rank

            var n = sorted.Count;
            if (n < 5)
                return null;

            // to check 34567 to XJQKA
            for (int i = 0; i < n - 5; i++)
            {
                if (sorted[i].Rank == sorted[i + 1].Rank - 1
                    && sorted[i].Rank == sorted[i + 2].Rank - 2
                    && sorted[i].Rank == sorted[i + 3].Rank - 3
                    && sorted[i].Rank == sorted[i + 4].Rank - 4)
                {
                    return new Hand(sorted[i], sorted[i + 1], sorted[i + 2], sorted[i + 3], sorted[i + 4]);
                }
            }

            // to check 2345A
            var first = sorted[0];
            if (first.Rank == (Ranks)2)
            {
                if (sorted[1].Rank == (Ranks)3
                    && sorted[2].Rank == (Ranks)4
                    && sorted[3].Rank == (Ranks)5
                    && sorted[n - 1].Rank == Ranks.A)
                {
                    return new Hand(first, sorted[1], sorted[2], sorted[3], sorted[n - 1]);
                }
            }

            return null;
        }

        public static bool TestStraight(List<Card> sortedFiveCards)
        {
            if (sortedFiveCards.Count != 5)
                return false;

            if (sortedFiveCards[0].Rank != (Ranks)2
                && sortedFiveCards[0].Rank == sortedFiveCards[0 + 1].Rank - 1
                && sortedFiveCards[0].Rank == sortedFiveCards[0 + 2].Rank - 2
                && sortedFiveCards[0].Rank == sortedFiveCards[0 + 3].Rank - 3
                && sortedFiveCards[0].Rank == sortedFiveCards[0 + 4].Rank - 4)
            {
                return true;
            }
            if (sortedFiveCards[0].Rank == (Ranks)2
                && sortedFiveCards[1].Rank == (Ranks)3
                && sortedFiveCards[2].Rank == (Ranks)4
                && sortedFiveCards[3].Rank == (Ranks)5
                && sortedFiveCards[4].Rank == Ranks.A)
            {
                return true;
            }
            return false;
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