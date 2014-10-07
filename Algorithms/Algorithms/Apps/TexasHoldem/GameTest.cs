namespace Algorithms.Apps.TexasHoldem
{
    public class GameTest
    {
        public static void Start()
        {
            var game = new Game();
            game.AddPlayers(6);
            game.Players[0].IsSmallBlind = true;
            game.Players[1].IsBigBlind = true;

            game.IsStarted = true;

            for (int i = 0; i < 10; i++)
            {
                PlayOneRound(game);

                game.ShiftBlinds();
            }
        }

        public static void PlayOneRound(Game game)
        {
            game.PrepareCards();

            // the beginning
            game.DealCards();
            game.BetOrFold();
            if (game.ActivePlayerCount == 1)
            {
                game.Payout();
                return;
            }

            // first 3 cards
            game.DealFlop();
            game.BetOrFold();
            if (game.ActivePlayerCount == 1)
                return;

            // next 4th card
            game.DealTurn();
            game.BetOrFold();
            if (game.ActivePlayerCount == 1)
                return;

            // next 5th card
            game.DealRiver();
            game.BetOrFold();
            if (game.ActivePlayerCount == 1)
                return;

            // settle the winner
            game.Showdown();
        }
    }
}