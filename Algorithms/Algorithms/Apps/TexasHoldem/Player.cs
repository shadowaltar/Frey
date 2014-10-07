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
        public Hand BestPokerHand { get; private set; }

        public bool IsFolded { get; set; }

        public int Money { get; set; }

        /// <summary>
        /// How many times the player places a bet on the table in current round.
        /// </summary>
        public int CurrentRoundBet { get; private set; }
        public int AllRoundBet { get; private set; }

        public List<int[]> BetHistory { get; set; }

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

        public BetAction Bet(int betRound, int previousPlayerBet)
        {
            if (betRound == 1)
            {
                if (IsSmallBlind)
                {
                    var bet = game.MinimumBet / 2;
                    Money -= bet;
                    CurrentRoundBet = bet;
                    AllRoundBet += bet;
                    game.BetOnTheTable += bet;
                    return BetAction.SmallBlind;
                }
                if (IsBigBlind)
                {
                    var bet = game.MinimumBet;
                    Money -= bet;
                    CurrentRoundBet = bet;
                    AllRoundBet += bet;
                    game.BetOnTheTable += bet;
                    return BetAction.BigBlind;
                }
                else
                {
                    Think();
                    if (CardPower == CardPower.Ok)
                    {

                    }
                }
            }

            if (game.BetRound == 1)
            {
                if (IsSmallBlind)
                {
                    var bet = game.MinimumBet / 2;
                    Money -= bet;
                    CurrentRoundBet = bet;
                    AllRoundBet += bet;
                    game.BetOnTheTable += bet;

                }
                else if (IsBigBlind)
                {
                    var bet = game.MinimumBet;
                    Money -= bet;
                    CurrentRoundBet = bet;
                    AllRoundBet += bet;
                    game.BetOnTheTable += bet;
                }
            }
            switch (action)
            {
                case BetAction.Check:
                    {
                        CurrentRoundBet = 0;
                        break;
                    }
                case BetAction.Raise:
                    {
                        int bet = CalculateBetAmount(previousPlayerBet);

                        Money -= bet;
                        CurrentRoundBet += bet;
                        AllRoundBet += bet;
                        game.BetOnTheTable += bet;
                        break;
                    }
            }
        }

        /// <summary>
        /// TODO: naive algo.
        /// </summary>
        /// <param name="previousPlayerBet"></param>
        /// <returns></returns>
        protected virtual int CalculateBetAmount(int previousPlayerBet)
        {
            return game.MinimumBet + previousPlayerBet;
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
}