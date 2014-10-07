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
            CurrentRoundPlayers = new List<Player>();

            Flop = new Deck();
            CardsOnTheTable = new Deck();

            RaiseRule = RaiseRule.OneRaise;
            SmallBlindPosition = 0;

            MinimumBet = 10;
        }

        public bool IsStarted { get; set; }

        public int MinimumBet { get; set; }

        public int BetOnTheTable { get; set; }

        public int CardRound { get; set; }
        public int BetRound { get; set; }

        public List<Player> Players { get; private set; }
        public List<Player> CurrentRoundPlayers { get; private set; }

        public int SmallBlindPosition { get; private set; }
        public int FirstPlayerPosition { get; private set; }
        public int PlayerCount { get { return Players.Count; } }
        public int FoldedPlayerCount { get { return Players.Count(p => p.IsFolded); } }
        public int ActivePlayerCount { get { return Players.Count - FoldedPlayerCount; } }

        public RaiseRule RaiseRule { get; protected set; }

        public Player FirstPlayer { get { return Players[FirstPlayerPosition]; } }
        public Player LastPlayer { get { return Players[(FirstPlayerPosition - 1 + PlayerCount) % PlayerCount]; } }

        /// <summary>
        /// All initial cards.
        /// </summary>
        public Deck Deck { get; set; }
        /// <summary>
        /// Initial 3 cards on the table.
        /// </summary>
        public Deck Flop { get; private set; }
        /// <summary>
        /// The 4th card on the table.
        /// </summary>
        public Card Turn { get; private set; }
        /// <summary>
        /// The 5th card on the table.
        /// </summary>
        public Card River { get; private set; }

        public Deck CardsOnTheTable { get; private set; }

        public void AddPlayer()
        {
            if (IsStarted)
                throw new InvalidOperationException();

            var newPlayer = new Player("Player " + (PlayerCount + 1), this);
            Players[PlayerCount - 1].NextPlayer = newPlayer;
            Players.Add(newPlayer);
            newPlayer.NextPlayer = Players[0];
        }

        public void AddPlayers(int count)
        {
            if (IsStarted)
                throw new InvalidOperationException();

            for (int i = 0; i < count; i++)
            {
                Players.Add(new Player("Player " + (i + 1), this));

                if (i > 0)
                    Players[i - 1].NextPlayer = Players[i];
            }
            Players[PlayerCount - 1].NextPlayer = Players[0];
        }

        public void PrepareCards()
        {
            Deck = Deck.NewRandom();
        }

        public void DealCards()
        {
            // deal another round of cards before first bet
            Deal();
            Deal();
            CardRound++;
        }

        private void Burn()
        {
            Deck.Draw(); // burn one card
        }

        private void Deal()
        {
            foreach (var player in Players)
            {
                player.AddHandCard(Deck.Draw());
            }
        }

        public void DealFlop()
        {
            Flop.Add(Deck.Draw());
            Flop.Add(Deck.Draw());
            Flop.Add(Deck.Draw());
            CardsOnTheTable.Add(Flop);
        }

        public void DealTurn()
        {
            Burn();
            var card = Deck.Draw();
            CardsOnTheTable.Add(card);
            Turn = card;
        }

        public void DealRiver()
        {
            Burn();
            var card = Deck.Draw();
            CardsOnTheTable.Add(card);
            River = card;
        }

        public void BetOrFold()
        {
            var round = 1;
            var current = FirstPlayer;
            var previous = LastPlayer;
            do
            {
                var action = current.Bet(round, 0);
                if (action != BetAction.Fold)
                {
                    previous = current;
                    current = current.NextPlayer;
                }
                else // when this user folds
                {
                    previous.NextPlayer = current.NextPlayer;
                    if (current == FirstPlayer)
                        LastPlayer.NextPlayer = current.NextPlayer;
                }
            } while (current != FirstPlayer);

            if (FirstPlayer.CurrentRoundBetCount != LastPlayer.CurrentRoundBet)
            {
                // 2nd bet round.
                round = 2;
                do
                {
                    current.Bet(round, 0);
                    current = current.NextPlayer;
                } while (current != FirstPlayer);
            }
        }

        public void Showdown()
        {
            Player bestPlayer = null;
            foreach (var player in Players.Where(p => !p.IsFolded))
            {
                //if (bestPlayer == null || !bestPlayer.BestPokerHand.StrongerThan(player.BestPokerHand))
                //    bestPlayer = player;
            }
        }

        public void ShiftBlinds()
        {
            var smallBlindPosition = Players.FindIndex(p => p.IsSmallBlind);
            var bigBlindPosition = Players.FindIndex(p => p.IsSmallBlind);

            Players[smallBlindPosition].IsSmallBlind = false;
            Players[smallBlindPosition].NextPlayer.IsSmallBlind = true;
            Players[bigBlindPosition].IsBigBlind = false;
            Players[bigBlindPosition].NextPlayer.IsBigBlind = true;
        }

        public void Payout()
        {

        }
    }
}