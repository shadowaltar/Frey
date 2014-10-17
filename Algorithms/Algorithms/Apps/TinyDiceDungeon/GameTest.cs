using System;

namespace Algorithms.Apps.TinyDiceDungeon
{
    public class GameTest
    {
        public static void PlayerTest()
        {
            for (int i = 1; i <= 15; i++)
            {
                var game = new Game();
                double result = game.EstimateOdds(i, Game.DoubleOneNormalTwoTripler, 1000000);
                Console.WriteLine("Result when throw {0} times: {1}", i, result);
            }
        }
    }
}