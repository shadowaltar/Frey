using System;
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
                return new[] { CardPower.Ok, CardPower.Ok };
            return new[] { CardPower.Ok, CardPower.Ok };
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

        public static List<long> TimeUsed { get; private set; }

        /// <summary>
        /// Try to find out possible 5-card combinations from a hand with 5/6/7 cards.
        /// </summary>
        /// <param name="cards"></param>
        /// <returns></returns>
        public static Hand Find(Cards cards)
        {
            TimeUsed = new List<long>();

            if (cards.Count < 5)
                return null;

            // order by suit, find flush (biggest)
            var hand = FindFlush(cards);
            var flushHand = hand;

            // step 1. straight flush
            var isStraightFlush = hand != null && TestStraight(hand.Cards);
            if (isStraightFlush)
            {
                hand.Set(HandType.StraightFlush, hand[0].Rank);
                return hand;
            }

            // (intermediate) find 4-of-a-kind/fullhouse/3-of-a-kind
            var rankGroupCounts = new Dictionary<Ranks, int>();
            foreach (var card in cards)
            {
                var r = card.Rank;
                if (!rankGroupCounts.ContainsKey(r))
                    rankGroupCounts[r] = 1;
                else
                    rankGroupCounts[r]++;
            }
            var fours = new List<Ranks>();
            var threes = new List<Ranks>();
            var twos = new List<Ranks>();
            var singles = new List<Ranks>();
            foreach (var pair in rankGroupCounts)
            {
                switch (pair.Value)
                {
                    case 4: fours.Add(pair.Key); break;
                    case 3: threes.Add(pair.Key); break;
                    case 2: twos.Add(pair.Key); break;
                    case 1: singles.Add(pair.Key); break;
                }
            }

            // step 2. get the 4-of-a-kind
            if (fours.Count > 0)
            {
                fours.Sort(); // last one is the biggest rank now
                var bigRank = fours[fours.Count - 1];
                hand = new Hand(Cards.GetCards(bigRank));
                if (singles.Count > 0)
                    hand.Add(cards.FirstOrDefault(c => c.Rank == singles[0]));
                else if (twos.Count > 0)
                    hand.Add(cards.FirstOrDefault(c => c.Rank == twos[0]));
                else if (threes.Count > 0)
                    hand.Add(cards.FirstOrDefault(c => c.Rank == threes[0]));
                hand.Set(HandType.FourOfAKind, bigRank);
                return hand;
            }

            // step 3. get fullhouse
            if (threes.Count > 1 || (threes.Count == 1 && twos.Count > 0))
            {
                threes.Sort();
                var bigRank = threes[threes.Count - 1];
                hand = new Hand(cards.Where(c => c.Rank == bigRank));
                if (twos.Count > 0)
                {
                    var pairRank = twos[0];
                    hand.AddRange(cards.Where(c => c.Rank == pairRank));
                    hand.Set(HandType.FullHouse, bigRank);
                    return hand;
                }
                if (threes.Count > 1)
                {
                    var pairRank = threes[0]; // we have 2 3-of-a-kinds: smaller one is used as pair.
                    hand.AddRange(cards.Where(c => c.Rank == pairRank).Take(2));
                    hand.Set(HandType.FullHouse, bigRank);
                    return hand;
                }
            }

            // step 4. flush
            if (flushHand != null)
            {
                flushHand.Set(HandType.Flush, flushHand[0].Rank);
                return flushHand;
            }

            // step 5. straight
            hand = FindStraight(cards);
            if (hand != null)
            {
                hand.Set(HandType.Straight, hand[0].Rank);
                return hand;
            }

            // step 6. 3-of-a-kind
            if (threes.Count == 1 && twos.Count == 0)
            {
                var bigRank = threes[0];
                hand = new Hand(cards.Where(c => c.Rank == bigRank));
                singles.Sort();
                var sn = singles.Count;
                hand.Add(cards.FirstOrDefault(c => c.Rank == singles[sn - 1]));
                hand.Add(cards.FirstOrDefault(c => c.Rank == singles[sn - 2]));
                hand.Set(HandType.ThreeOfAKind, bigRank);
                return hand;
            }

            // step 7. 2 pairs
            if (twos.Count > 1)
            {
                twos.Sort();
                var bigRank = twos[twos.Count - 1];
                var smallRank = twos[twos.Count - 2];
                hand = new Hand();
                hand.AddRange(cards.Where(c => c.Rank == bigRank));
                hand.AddRange(cards.Where(c => c.Rank == smallRank));
                hand.Add(cards.FirstOrDefault(c => c.Rank == singles[0]));
                hand.Set(HandType.TwoPair, bigRank);
                return hand;
            }

            // step 8. 1 pair
            if (twos.Count == 1)
            {
                hand = new Hand();
                hand.AddRange(cards.Where(c => c.Rank == twos[0]));
                singles.Sort();
                var sn = singles.Count;
                hand.Add(cards.FirstOrDefault(c => c.Rank == singles[sn - 1]));
                hand.Add(cards.FirstOrDefault(c => c.Rank == singles[sn - 2]));
                hand.Add(cards.FirstOrDefault(c => c.Rank == singles[sn - 3]));
                hand.Set(HandType.OnePair, twos[0]);
                return hand;
            }

            // last step. high card
            hand = new Hand(cards.OrderByDescending(s => s).Take(5));
            hand.Set(HandType.HighCard, hand.Cards[0].Rank);

            return hand;
        }

        /// <summary>
        /// Returns the first flush, with maximum ranks.
        /// </summary>
        /// <param name="cards"></param>
        /// <returns></returns>
        private static Hand FindFlush(Cards cards)
        {
            var n = cards.Count;
            if (n < 5)
                return null;
            foreach (var suit in typeof(Suits).Values(Suits.Any))
            {
                var suitCards = cards.Where(c => c.Suit == suit).ToArray();
                if (suitCards.Length == 5)
                {
                    return new Hand(suitCards);
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

        public static bool TestStraight(List<Card> sortedFiveCards, int offset = 0)
        {
            if (sortedFiveCards.Count < 5)
                return false;

            if (sortedFiveCards[offset].Rank != (Ranks)2
                && sortedFiveCards[offset].Rank == sortedFiveCards[offset + 1].Rank - 1
                && sortedFiveCards[offset].Rank == sortedFiveCards[offset + 2].Rank - 2
                && sortedFiveCards[offset].Rank == sortedFiveCards[offset + 3].Rank - 3
                && sortedFiveCards[offset].Rank == sortedFiveCards[offset + 4].Rank - 4)
            {
                return true;
            }
            if (sortedFiveCards[offset].Rank == (Ranks)2
                && sortedFiveCards[offset + 1].Rank == (Ranks)3
                && sortedFiveCards[offset + 2].Rank == (Ranks)4
                && sortedFiveCards[offset + 3].Rank == (Ranks)5
                && sortedFiveCards[offset + 4].Rank == Ranks.A)
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
                hand.SignificantRank = hand[0].Rank;
            }

            // check all non-flush
            if (hand[1].Rank + 1 == hand[0].Rank
                && hand[2].Rank + 1 == hand[1].Rank
                && hand[3].Rank + 1 == hand[2].Rank
                && hand[4].Rank + 1 == hand[3].Rank)
            {
                isStraight = true;
                hand.SignificantRank = hand[0].Rank;
            }
            else if (hand[0].Rank == Ranks.A
                     && hand[1].Rank == Ranks.Five
                     && hand[2].Rank == Ranks.Four
                     && hand[3].Rank == Ranks.Three
                     && hand[4].Rank == Ranks.Two)
            {
                isStraight = true;
                hand.SignificantRank = Ranks.Five;
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
                                    hand.SignificantRank = hand[1].Rank;
                                    break;
                                case 4:
                                    hand.Type = HandType.FourOfAKind;
                                    hand.SignificantRank = hand[0].Rank;
                                    break;
                                case 2:
                                    hand.Type = HandType.FullHouse;
                                    hand.SignificantRank = hand[1].Rank;
                                    break;
                                case 3:
                                    hand.Type = HandType.FullHouse;
                                    hand.SignificantRank = hand[0].Rank;
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
                                hand.SignificantRank = hand[2].Rank; // the mid one always have 3.
                            }
                            else
                            {
                                hand.Type = HandType.TwoPair;
                                hand.SignificantRank = first == 1 ? hand[2].Rank : hand[0].Rank;
                            }
                            break;
                        }
                    case 4: // 1pair
                        {
                            hand.Type = HandType.OnePair;
                            hand.SignificantRank = groups[hand[1].Rank] == 2 ? hand[1].Rank : hand[3].Rank;
                            break;
                        }
                    case 5: // high card (ex. straights)
                        {
                            hand.Type = HandType.HighCard;
                            hand.SignificantRank = hand[0].Rank;
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

        public static int Compare(Hand one, Hand two)
        {
            if (one.Count < 5 || two.Count < 5)
                throw new InvalidOperationException();

            // diff types
            if (one.Type > two.Type)
                return 1;
            if (one.Type < two.Type)
                return -1;

            // same type
            switch (one.Type)
            {
                case HandType.StraightFlush:
                case HandType.FourOfAKind:
                case HandType.FullHouse:
                case HandType.Straight:
                    return one.SignificantRank.CompareTo(two.SignificantRank);
                case HandType.ThreeOfAKind:
                    {
                        var r = one.Cards[0].CompareTo(two.Cards[0]); // the '3'
                        if (r != 0) return r;
                        r = one.Cards[3].CompareTo(two.Cards[3]); // 1st single
                        if (r != 0) return r;
                        return one.Cards[4].CompareTo(two.Cards[4]); // 2nd single
                    }
                case HandType.Flush:
                    return CompareFlushRank(one, two);
                case HandType.TwoPair:
                    {
                        var r = one.Cards[0].CompareTo(two.Cards[0]); // 1st pair
                        if (r != 0) return r;
                        r = one.Cards[2].CompareTo(two.Cards[2]); // 2nd pair
                        if (r != 0) return r;
                        return one.Cards[4].CompareTo(two.Cards[4]); // the single
                    }
                case HandType.OnePair:
                    {
                        var r = one.Cards[0].CompareTo(two.Cards[0]); // 1st pair
                        if (r != 0) return r;

                        for (int i = 2; i < 5; i++)
                        {
                            r = one.Cards[i].CompareTo(two.Cards[i]); // 2nd pair
                            if (r != 0) return r;
                        }
                        return r;
                    }
                case HandType.HighCard:
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            var r = one.Cards[i].CompareTo(two.Cards[i]); // 2nd pair
                            if (r != 0)
                                return r;
                        }
                        return 0;
                    }
            }
            return 0;
        }
    }
}