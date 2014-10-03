using System.Collections.Generic;
using Algorithms.Algos;

namespace Algorithms.Apps.TexasHoldem
{
    public class Player
    {
        public Player(string name, Game game)
        {
            this.game = game;
            Name = name;

            Money = 10000;

            HandCards = new List<Card>();
            AllCards = new List<Card>();
            BestPokerHand = null;
        }

        public string Name { get; set; }

        public List<Card> HandCards { get; private set; }
        public List<Card> AllCards { get; private set; }
        public List<Card> BestPokerHand { get; private set; }

        public bool IsFolded { get; set; }

        public int Money { get; set; }

        /// <summary>
        /// How many times the player places a bet on the table in current round.
        /// </summary>
        public int CurrentRoundBet { get; private set; }
        public int AllRoundBet { get; private set; }

        /// <summary>
        /// How many times the player places a bet on the table in current round.
        /// </summary>
        public int CurrentRoundBetCount { get; private set; }

        /// <summary>
        /// Small blind is the player who receive card first.
        /// </summary>
        public bool IsSmallBlind { get; set; }
        /// <summary>
        /// Small blind is the player who receive card next.
        /// </summary>
        public bool IsBigBlind { get; set; }

        public Player NextPlayer { get; set; }
        public CardPower CardPower { get; private set; }
        public CardPower AverageOpponentCardPower { get; private set; }

        private readonly Game game;

        public void NextRound()
        {
            CurrentRoundBet = 0;
            CurrentRoundBetCount = 0;
        }

        public void AddHandCard(Card draw)
        {
            HandCards.Add(draw);
            HandCards = Sortings.QuickSort(HandCards);
        }

        public int Bet(BetAction action = BetAction.Check)
        {
            var bet = 0;
            if (action == BetAction.Check)
            {
                bet = game.MinimumBet;
                Money -= bet;
                CurrentRoundBet += bet;
                AllRoundBet += bet;
            }

            return bet;
        }

        public void Think()
        {
            var results = CardCombinationHelper.Calculate(game.CardsOnTheTable, game.CardRound, HandCards);
            CardPower = results[0];
            AverageOpponentCardPower = results[1];
        }

        public void Fold()
        {
            IsFolded = true;
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
            return string.Format("{0} {1}, {2}, has {3}", Name, HandCardsToString(), IsFolded ? "folded" : "playing", Money);
        }
    }

    public enum CardPower
    {
        Weak,
        Ok,
        Strong,
    }

    public enum BetAction
    {
        Raise,
        Follow,
        Check,
    }
}