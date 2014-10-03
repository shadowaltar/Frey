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
            Flop = new Deck();
            CardsOnTheTable = new Deck();

            MinimumBet = 10;
        }

        public bool IsStarted { get; set; }

        public int MinimumBet { get; set; }

        public int BetOnTheTable { get; set; }

        public int CardRound { get; set; }
        public int BetRound { get; set; }

        public List<Player> Players { get; private set; }

        public int PlayerCount { get { return Players.Count; } }
        public int FoldedPlayerCount { get { return Players.Count(p => p.IsFolded); } }
        public int ActivePlayerCount { get { return Players.Count - FoldedPlayerCount; } }

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
            foreach (var player in Players)
            {
                // simple naive bet (all players place min bet)
                player.Think();
                if (player.CardPower == CardPower.Weak)
                {
                    player.Fold();
                }
                else if (player.CardPower == CardPower.Ok)
                {
                    BetOnTheTable += player.Bet();
                }
                else if (player.CardPower == CardPower.Strong)
                {
                    BetOnTheTable += player.Bet(BetAction.Raise);
                }
            }
        }

        public void Showdown()
        {
            Player bestPlayer = null;
            foreach (var player in Players.Where(p => !p.IsFolded))
            {
                if (bestPlayer == null || !bestPlayer.BestPokerHand.StrongerThan(player.BestPokerHand))
                    bestPlayer = player;
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
    }
}