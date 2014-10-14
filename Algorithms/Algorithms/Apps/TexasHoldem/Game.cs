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
            CardsOnTable = new Cards();
            GameHistory = new List<GameRound>();
        }

        public bool IsStarted { get; set; }

        public int CurrentGameRound { get; set; }

        public bool HasGameRoundWinner { get { return Players.Count(p => p.IsActive) == 1; } }

        public int MinimumBet { get; set; }

        public double Pot { get; set; }

        public int CardRound { get; set; }

        public List<GameRound> GameHistory { get; private set; }

        public List<Player> Players { get; private set; }
        public List<Player> ActivePlayers { get { return Players.Where(p => p.IsActive).ToList(); } }
        public Player CurrentFirstPlayer { get; private set; }
        public Player CurrentSmallBlind { get; private set; }

        public int PlayerCount { get { return Players.Count; } }
        public int ActivePlayerCount { get { return Players.Count(p => p.IsActive); } }

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

        public Cards CardsOnTable { get; private set; }

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
            var winners = ActivePlayers;
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
                            winners = Showdown().ToList();
                        }
                    }
                }
            }
            Payout(winners);
            Record(winners);
            SetBlinds();
        }

        private void Record(List<Player> winners)
        {
            foreach (var player in ActivePlayers)
            {
                Console.WriteLine("{0} has money: {1}", player.Name, player.CurrentMoney);
            }

            var gr = new GameRound(CurrentGameRound, CardsOnTable);
            foreach (var player in Players)
            {
                var pgr = new PlayerGameRound();
                pgr.BestHand = player.BestHand;
                pgr.Bet = player.BetOfGameRound;
                pgr.CurrentMoney = player.CurrentMoney;
                pgr.CardsHeld = new Cards(player.HandCards);

                if (winners.Contains(player))
                {
                    pgr.IsWinner = true;
                    pgr.ProfitLoss = Pot / winners.Count;
                }
                else
                {
                    pgr.ProfitLoss = -player.BetOfGameRound;
                }
                gr.Players.Add(pgr);
            }
            GameHistory.Add(gr);
        }

        public void PrepareCards()
        {
            Deck = Deck.NewRandom();
        }

        public void InitializeRound()
        {
            CurrentGameRound++;
            CurrentFirstPlayer = CurrentSmallBlind;

            foreach (var player in Players)
            {
                player.InitializeGameRound();
            }

            Pot = 0;
            CardsOnTable.Clear();
            Flop.Clear();
            Turn = null;
            River = null;
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
            CardsOnTable.AddRange(Flop);
        }

        public void DealTurn()
        {
            Burn();
            Turn = Deck.Draw();
            CardsOnTable.Add(Turn);
            CardsOnTable.Sort();
        }

        public void DealRiver()
        {
            Burn();
            River = Deck.Draw();
            CardsOnTable.Add(River);
            CardsOnTable.Sort();
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

        public HashSet<Player> Showdown()
        {
            var players = ActivePlayers;
            players.ForEach(a => a.SortCards());
            var strongest = players[0];
            var results = new HashSet<Player>();
            for (int i = 1; i < players.Count; i++)
            {
                var current = players[i];

                var r = strongest.BestHand.CompareTo(current.BestHand);
                if (r == 0)
                {
                    results.Add(strongest);
                    results.Add(current);
                }
                else if (r < 0)
                {
                    strongest = current;
                    results.Clear();
                }
            }

            if (results.Count == 0)
            {
                results.Add(strongest);
            }

            Console.WriteLine("[CARDS ON TABLE] {0}", CardsOnTable);
            foreach (var player in players)
            {
                Console.WriteLine("{0} has {1}; best hand: {2}", player.Name, player.HandCards, player.BestHand);
            }
            foreach (var winner in results)
            {
                Console.WriteLine("[WINNER] {0} {1}", winner.Name, winner.BestHand);
            }
            return results;
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

        public void Payout(List<Player> winners)
        {
            foreach (var winner in winners)
            {
                var winAmount = Pot / winners.Count;
                winner.Win(winAmount);

                Console.WriteLine("{0} earns {1}!", winner.Name, winAmount);
            }
        }
    }

    public class GameRound
    {
        public GameRound(int round, Cards cardsOnTable)
        {
            Players = new List<PlayerGameRound>();
            Round = round;
            CardsOnTable = cardsOnTable;
        }

        public int Round { get; private set; }
        public Cards CardsOnTable { get; private set; }
        public List<PlayerGameRound> Players { get; private set; }
    }

    public class PlayerGameRound
    {
        public bool IsWinner { get; set; }
        public Cards CardsHeld { get; set; }
        public Hand BestHand { get; set; }
        public int Bet { get; set; }
        public double ProfitLoss { get; set; }
        public double CurrentMoney { get; set; }
    }
}