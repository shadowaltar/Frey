namespace Algorithms.Apps.TexasHoldem
{
    public class GameTest
    {
        public static void Start()
        {
            var game = new Game();
            game.Initialize(4);

            game.IsStarted = true;

            for (int i = 0; i < 10; i++)
            {
                game.PlayOneRound();
            }
        }
    }
}