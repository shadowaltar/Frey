using Algorithms.Utils;

namespace Algorithms.Apps.TinyDiceDungeon
{
    public class Game
    {
        public double EstimateOdds(int throwCount, int experimentCount)
        {
            double totalValue = 0;
            var value = 0;

            for (int j = 0; j < experimentCount; j++)
            {
                for (int i = 0; i < throwCount; i++)
                {
                    var result = StaticRandom.Instance.Next(1, 7);
                    if (result == 1)
                    {
                        value = 0;
                        break;
                    }
                    value += result;
                }
                totalValue += value;
                value = 0;
            }
            return totalValue / experimentCount;
        }
    }
}