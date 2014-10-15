using System;

namespace Algorithms.Apps.TinyDiceDungeon
{
    public class GameTest
    {
        public static void Test()
        {
            for (int i = 1; i <= 15; i++)
            {
                var game = new Game();
                var result = game.EstimateOdds(i, 1000000);
                Console.WriteLine("Result when throw {0} times: {1}", i, result);
            }
        }
    }
}