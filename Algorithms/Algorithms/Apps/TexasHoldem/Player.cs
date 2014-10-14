using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Algorithms.Apps.TexasHoldem
{
    public class Player
    {
        public Player(string name, Game game, int money)
        {
            this.game = game;
            Name = name;

            InitialMoney = money;
            CurrentMoney = money;

            HandCards = new Cards();
            BestHand = null;

            IsFolded = false;
            IsLoss = false;
        }

        public string Name { get; set; }

        public Cards HandCards { get; private set; }
        public Cards AllCards { get { return GetAllCards(); } }

        public Hand BestHand { get; set; }

        public bool IsFolded { get; set; }
        public bool IsLoss { get; set; }
        public bool IsActive { get { return !IsFolded && !IsLoss; } }
        public BetAction LatestBetAction { get; set; }

        public double InitialMoney { get; set; }

        public int AddedBetOfFirstBetRound { get; private set; }
        public int AddedBetOfSecondBetRound { get; private set; }
        public int BetOfCardRound { get { return AddedBetOfFirstBetRound + AddedBetOfSecondBetRound; } }
        public int BetOfGameRound { get; private set; }

        public double CurrentMoney { get; private set; }

        public List<double[]> BetHistory { get; set; }

        /// <summary>
        /// Small blind is the player who receive card first.
        /// </summary>
        public bool IsSmallBlind { get; set; }
        /// <summary>
        /// Small blind is the player who receive card next.
        /// </summary>
        public bool IsBigBlind { get; set; }

        /// <summary>
        /// Player sitting next to him, fixed at the very beginning unless he quits the game (bankrupt).
        /// Don't change this by folding.
        /// </summary>
        public Player NextPlayer { get { return GetNextPlayer(); } }
        public Player NextActivePlayer { get { return GetNextActivePlayer(); } }
        public Player PreviousActivePlayer { get { return GetPreviousActivePlayer(); } }

        public CardPower CardPower { get; private set; }
        public CardPower AverageOpponentCardPower { get; private set; }

        private readonly Game game;

        public void InitializeGameRound()
        {
            AddedBetOfFirstBetRound = 0;
            AddedBetOfSecondBetRound = 0;
            BetOfGameRound = 0;

            HandCards.Clear();
            AllCards.Clear();
            BestHand = null;

            LatestBetAction = BetAction.NotDecided;

            IsFolded = false;
            IsLoss = CurrentMoney == 0;

            CardPower = CardPower.Unknown;
            AverageOpponentCardPower = CardPower.Unknown;
        }

        public void InitializeCardRound()
        {
            AddedBetOfFirstBetRound = 0;
            AddedBetOfSecondBetRound = 0;

            LatestBetAction = BetAction.NotDecided;

            IsLoss = CurrentMoney == 0;

            CardPower = CardPower.Unknown;
            AverageOpponentCardPower = CardPower.Unknown;
        }

        public void AddHandCard(Card card)
        {
            HandCards.Add(card);
            if (HandCards.Count > 1)
                HandCards.Sort();
        }

        /// <summary>
        /// The player bets, raises or folds.
        /// </summary>
        /// <param name="cardRound">Round count when cards are dealt by dealer (start at 1).</param>
        /// <param name="betRound">Round count within each <see cref="cardRound"/>, when
        /// players place bet (start at 1).</param>
        /// <returns></returns>
        public BetAction Bet(int cardRound, int betRound)
        {
            LatestBetAction = BetAction.NotDecided;

            // round 1 only
            if (betRound == 1)
            {
                if (cardRound == 1 && IsSmallBlind)
                {
                    var bet = game.MinimumBet / 2;
                    AddBet(betRound, bet);

                    Console.WriteLine("CR {0}, BR {1}: {2} adds bet {3}; CR total bets {4}; GR total bets {5}",
                        cardRound, betRound, Name, bet, BetOfCardRound, BetOfGameRound);
                    LatestBetAction = BetAction.SmallBlind;
                }
                else if (cardRound == 1 && IsBigBlind)
                {
                    var bet = game.MinimumBet;
                    AddBet(1, bet);

                    Console.WriteLine("CR {0}, BR {1}: {2} adds bet {3}; CR total bets {4}; GR total bets {5}",
                        cardRound, betRound, Name, bet, BetOfCardRound, BetOfGameRound);
                    LatestBetAction = BetAction.BigBlind;

                }
                else if (game.CurrentFirstPlayer == this)
                {
                    // you are the first player of this non-1st card round (round without blind betting)
                    Think();
                    if (CardPower == CardPower.Strong)
                    {
                        // you are the first, simply add bet.
                        var bet = game.MinimumBet;
                        AddBet(betRound, bet);

                        Console.WriteLine("CR {0}, BR {1}: {2} adds bet {3}; CR total bets {4}; GR total bets {5}",
                            cardRound, betRound, Name, bet, BetOfCardRound, BetOfGameRound);
                        LatestBetAction = BetAction.Raise;
                    }
                    else // if (CardPower == CardPower.Ok || CardPower == CardPower.Weak)
                    {
                        // check, whether weak or so-so
                        var bet = 0;
                        AddBet(betRound, bet);

                        Console.WriteLine("CR {0}, BR {1}: {2} adds bet {3}; CR total bets {4}; GR total bets {5}",
                            cardRound, betRound, Name, bet, BetOfCardRound, BetOfGameRound);
                        LatestBetAction = BetAction.Check;
                    }
                }
                else
                {
                    var previousPlayerBet = PreviousActivePlayer.BetOfCardRound;

                    Think();
                    if (CardPower == CardPower.Strong)
                    {
                        // double previous one's bet
                        var bet = previousPlayerBet * 2;
                        AddBet(betRound, bet);

                        Console.WriteLine("CR {0}, BR {1}: {2} adds bet {3}; CR total bets {4}; GR total bets {5}",
                            cardRound, betRound, Name, bet, BetOfCardRound, BetOfGameRound);
                        LatestBetAction = BetAction.Raise;
                    }
                    if (CardPower == CardPower.Ok)
                    {
                        // call or check (follow previous)
                        var bet = previousPlayerBet;
                        AddBet(betRound, previousPlayerBet);

                        Console.WriteLine("CR {0}, BR {1}: {2} adds bet {3}; CR total bets {4}; GR total bets {5}",
                            cardRound, betRound, Name, bet, BetOfCardRound, BetOfGameRound);
                        LatestBetAction = previousPlayerBet != 0 ? BetAction.Call : BetAction.Check;
                    }
                    if (CardPower == CardPower.Weak)
                    {
                        Fold();

                        Console.WriteLine("CR {0}, BR {1}: {2} folds", cardRound, betRound, Name);
                        LatestBetAction = BetAction.Fold;
                    }
                }
            }

            if (betRound == 2)
            {
                var previousPlayerBet = PreviousActivePlayer.BetOfCardRound;
                Think();
                if (CardPower == CardPower.Strong)
                {
                    // double previous one's bet, or add if previously 0.
                    var bet = previousPlayerBet == 0 ? game.MinimumBet : previousPlayerBet * 2 - AddedBetOfFirstBetRound;
                    // double previous one's bet
                    AddBet(2, bet);
                    Console.WriteLine("CR {0}, BR {1}: {2} adds bet {3}; CR total bets {4}; GR total bets {5}",
                        cardRound, betRound, Name, bet, BetOfCardRound, BetOfGameRound);
                    LatestBetAction = BetAction.Raise;
                }
                else if (CardPower == CardPower.Ok)
                {
                    // call (follow)
                    var bet = 0;
                    if (AddedBetOfFirstBetRound != previousPlayerBet)
                    {
                        bet = previousPlayerBet - AddedBetOfFirstBetRound;
                        AddBet(betRound, bet);
                        LatestBetAction = BetAction.Call;
                    }
                    else
                    {
                        LatestBetAction = BetAction.Check;
                    }
                    Console.WriteLine("CR {0}, BR {1}: {2} adds bet {3}; CR total bets {4}; GR total bets {5}",
                         cardRound, betRound, Name, bet, BetOfCardRound, BetOfGameRound);
                }
                else if (CardPower == CardPower.Weak)
                {
                    if (PreviousActivePlayer.LatestBetAction == BetAction.Check)
                    {
                        LatestBetAction = BetAction.Check;
                    }
                    else
                    {
                        Fold();
                        Console.WriteLine("CR {0}, BR {1}: {2} folds", cardRound, betRound, Name);
                        LatestBetAction = BetAction.Fold;
                    }
                }
            }

            if (LatestBetAction == BetAction.NotDecided)
                throw new InvalidOperationException();
            return LatestBetAction;
        }

        /// <summary>
        /// Add money to the table's bet pool. It is the increment bet amount.
        /// </summary>
        /// <param name="betRound"></param>
        /// <param name="amount"></param>
        private void AddBet(int betRound, int amount)
        {
            if (amount < 0 || betRound > 2)
                throw new ArgumentException();

            if (betRound == 1)
                AddedBetOfFirstBetRound = amount;
            else
                AddedBetOfSecondBetRound = amount;

            CurrentMoney -= amount;
            BetOfGameRound += amount;
            game.Pot += amount;
        }

        public void Think()
        {
            var results = CardCombinationHelper.Calculate(game.CardsOnTable, game.CardRound, HandCards);
            CardPower = results[0];
            AverageOpponentCardPower = results[1];
        }

        public void SortCards()
        {
            BestHand = CardCombinationHelper.Find(AllCards);
        }

        private Cards GetAllCards()
        {
            var cards = new Cards(game.CardsOnTable);
            cards.AddRange(HandCards);
            return cards;
        }

        private void Fold()
        {
            IsFolded = true;
        }

        private Player GetNextPlayer()
        {
            var position = game.Players.IndexOf(this);
            position = (position + 1) % game.PlayerCount;
            return game.Players[position];
        }

        private Player GetNextActivePlayer()
        {
            var position = game.Players.IndexOf(this);

            while (true)
            {
                position = (position + 1) % game.PlayerCount;
                if (!game.Players[position].IsActive)
                    continue;
                return game.Players[position];
            }
        }

        private Player GetPreviousActivePlayer()
        {
            var position = game.Players.IndexOf(this);

            while (true)
            {
                position = (position - 1 + game.PlayerCount) % game.PlayerCount;
                if (!game.Players[position].IsActive)
                    continue;
                return game.Players[position];
            }
        }

        public string HandCardsToString()
        {
            var str = "";
            foreach (var card in HandCards)
            {
                str += card + ",";
            }
            str = str.Trim(',');
            return str;
        }

        public override string ToString()
        {
            return string.Format("{0}; holds {1}; {2}, {3}, has {4}", Name, HandCardsToString(),
                BestHand == null ? "No best-hand" : "(" + BestHand.Type + ")" + BestHand.SignificantRank,
                IsFolded ? "folded" : "playing", CurrentMoney);
        }

        public void Win(double amount)
        {
            CurrentMoney += amount;
        }
    }
}