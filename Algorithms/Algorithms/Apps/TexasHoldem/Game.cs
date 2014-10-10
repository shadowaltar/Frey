using System;
using System.Collections.Generic;
using System.Linq;

namespace Algorithms.Apps.TexasHoldem
{
    public class Game
    {
        public Game()
        {
            Players = new List<Player>();

            Flop = new Cards();
            CardsOnTheTable = new Cards();

            RaiseRule = RaiseRule.OneRaise;

            MinimumBet = 10;
        }

        public bool IsStarted { get; set; }

        public bool HasGameRoundWinner { get { return Players.Count(p => p.IsActive) == 1; } }

        public int MinimumBet { get; set; }

        public int GameRoundBetPool { get; set; }

        public int CardRound { get; set; }

        public List<Player> Players { get; private set; }
        public List<Player> ActivePlayers { get { return Players.Where(p => p.IsActive).ToList(); } }
        public Player CurrentFirstPlayer { get; private set; }
        public Player CurrentSmallBlind { get; private set; }

        public int PlayerCount { get { return Players.Count; } }
        public int ActivePlayerCount { get { return Players.Count(p => p.IsActive); } }

        public RaiseRule RaiseRule { get; protected set; }

        /// <summary>
        /// All initial cards.
        /// </summary>
        public Deck Deck { get; set; }
        /// <summary>
        /// Initial 3 cards on the table.
        /// </summary>
        public Cards Flop { get; private set; }
        /// <summary>
        /// The 4th card on the table.
        /// </summary>
        public Card Turn { get; private set; }
        /// <summary>
        /// The 5th card on the table.
        /// </summary>
        public Card River { get; private set; }

        public Cards CardsOnTheTable { get; private set; }

        public void Initialize(int playerCount)
        {
            if (IsStarted)
                throw new InvalidOperationException();

            for (int i = 0; i < playerCount; i++)
            {
                Players.Add(new Player("Player " + (i + 1), this, 10000));
            }

            SetBlinds();
        }

        public void PlayOneRound()
        {
            PrepareCards();

            InitializeRound();

            // the beginning
            DealCards();
            var hasWinner = PlayCardRound(1);
            if (!hasWinner)
            {
                DealFlop();
                hasWinner = PlayCardRound(2);
                if (!hasWinner)
                {
                    DealTurn();
                    hasWinner = PlayCardRound(3);
                    if (!hasWinner)
                    {
                        DealRiver();
                        hasWinner = PlayCardRound(4);
                        if (!hasWinner)
                        {
                            Showdown();
                        }
                    }
                }
            }
            Payout();
            SetBlinds();
        }

        public void PrepareCards()
        {
            Deck = Deck.NewRandom();
        }

        public void InitializeRound()
        {
            CurrentFirstPlayer = CurrentSmallBlind;

            foreach (var player in Players)
            {
                player.InitializeGameRound();
            }

            GameRoundBetPool = 0;
        }

        public void DealCards()
        {
            // deal another round of cards before first bet
            var activePlayers = ActivePlayers;
            activePlayers.ForEach(p => p.AddHandCard(Deck.Draw()));
            activePlayers.ForEach(p => p.AddHandCard(Deck.Draw()));
            CardRound++;
        }

        private void Burn()
        {
            Deck.Draw(); // burn one card
        }

        public void DealFlop()
        {
            Flop.Add(Deck.Draw());
            Flop.Add(Deck.Draw());
            Flop.Add(Deck.Draw());
            Flop.Sort();
            CardsOnTheTable.AddRange(Flop);
        }

        public void DealTurn()
        {
            Burn();
            Turn = Deck.Draw();
            CardsOnTheTable.Add(Turn);
            CardsOnTheTable.Sort();
        }

        public void DealRiver()
        {
            Burn();
            River = Deck.Draw();
            CardsOnTheTable.Add(River);
            CardsOnTheTable.Sort();
        }

        private bool PlayCardRound(int cardRound)
        {
            foreach (var player in ActivePlayers)
            {
                player.InitializeCardRound();
            }

            Console.WriteLine("--Playing CR {0} BR {1}--", cardRound, 1);
            PlayBetRound(cardRound, 1);

            if (HasGameRoundWinner)
                return true;

            // find the 1st player in a bet round.
            CurrentFirstPlayer = CurrentSmallBlind;
            while (!CurrentFirstPlayer.IsActive)
            {
                CurrentFirstPlayer = CurrentFirstPlayer.NextPlayer;
            }

            Console.WriteLine("--Playing CR {0} BR {1}--", cardRound, 2);
            PlayBetRound(cardRound, 2);

            // assertion only
            AssertCardRoundAllSameBet();

            return HasGameRoundWinner;
        }

        private void AssertCardRoundAllSameBet()
        {
            var activePlayers = ActivePlayers;
            var cardRoundBet = activePlayers[0].BetOfCardRound;
            for (int i = 1; i < activePlayers.Count; i++)
            {
                var activePlayer = activePlayers[i];
                if (cardRoundBet != activePlayer.BetOfCardRound)
                    throw new InvalidOperationException();
            }
        }

        private void PlayBetRound(int cardRound, int betRound)
        {
            var current = CurrentFirstPlayer;
            var toPlayCount = ActivePlayerCount;
            var actionFinishedCount = 0;
            do
            {
                current.Bet(cardRound, betRound);
                current = current.NextActivePlayer;
                actionFinishedCount++;
            } while (actionFinishedCount != toPlayCount);
        }

        public void Showdown()
        {
            var bestPlayerAndHands = new Dictionary<Player, Cards>();
            foreach (var activePlayer in ActivePlayers)
            {
                CardCombinationHelper.Find(activePlayer.AllCards);
            }
        }

        public void SetBlinds()
        {
            if (ActivePlayerCount < 2)
                return;

            foreach (var player in Players)
            {
                player.IsSmallBlind = false;
                player.IsBigBlind = false;
            }

            if (CurrentSmallBlind == null)
            {
                CurrentSmallBlind = Players[0];
                CurrentSmallBlind.IsSmallBlind = true;
                CurrentSmallBlind.NextActivePlayer.IsBigBlind = true;
            }
            else
            {
                var next = CurrentSmallBlind.NextActivePlayer;
                next.IsSmallBlind = true;
                next.NextActivePlayer.IsBigBlind = true;
            }
        }

        public void Payout()
        {

        }
    }
}