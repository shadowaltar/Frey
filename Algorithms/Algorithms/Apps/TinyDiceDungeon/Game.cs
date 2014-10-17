using System;
using Algorithms.Utils;

namespace Algorithms.Apps.TinyDiceDungeon
{
    public class Game
    {
        public double EstimateOdds(int throwCount, Func<int, int> ordinaryOneDice, int experimentCount)
        {
            double totalValue = 0;
            for (int j = 0; j < experimentCount; j++)
            {
                totalValue += ordinaryOneDice(throwCount);
            }
            return totalValue / experimentCount;
        }

        public static int OneNormal(int throwCount)
        {
            var value = 0;
            for (int i = 0; i < throwCount; i++)
            {
                var result = StaticRandom.ThrowDice();
                if (result == 1)
                {
                    value = 0;
                    break;
                }
                value += result;
            }
            return value;
        }

        public static int TwoNormal(int throwCount)
        {
            var lastDiceValue = 0;
            var value = 0;
            for (int i = 1; i <= throwCount; i++)
            {
                var result = StaticRandom.ThrowDice();
                if (result == 1)
                {
                    value = 0;
                    break;
                }
                if (i % 2 == 0 && lastDiceValue == result)
                {
                    result *= 3;
                }
                else
                {
                    lastDiceValue = result;
                }
                value += result;
            }
            return value;
        }

        public static int DoubleOneNormalTwoTripler(int throwCount)
        {
            var sum = 0;
            if (throwCount % 2 == 0)
            {
                for (int i = 0; i < throwCount / 2; i++)
                {
                    var result = StaticRandom.ThrowDice(); // 1st, double-dice
                    if (result == 1)
                        return 0;
                    if (i % 2 == 0)
                        result *= 2;
                    int round = result;

                    result = StaticRandom.ThrowDice(); // 2nd, normal-dice
                    if (result == 1)
                        return 0;

                    round += result;
                    round *= StaticRandom.Instance.Next(1, 4); // multiplier dice 1-3,

                    sum += round;
                }
            }
            else
            {
                for (int i = 0; i < (throwCount - 1) / 2; i++)
                {
                    var result = StaticRandom.ThrowDice(); // 1st, double-dice
                    if (result == 1)
                        return 0;
                    if (i % 2 == 0)
                        result *= 2;
                    int round = result;

                    result = StaticRandom.ThrowDice(); // 2nd, normal-dice
                    if (result == 1)
                        return 0;

                    round += result;
                    round *= StaticRandom.Instance.Next(1, 4); // multiplier dice 1-3,

                    sum += round;
                }

                var result2 = StaticRandom.ThrowDice() * 2; // last throw, double-dice
                if (result2 == 1)
                    return 0;
                result2 *= StaticRandom.Instance.Next(1, 4); // multiplier dice 1-3
                sum += result2;
            }
            return sum;
        }

        public static int DoubleOneNormalTwo(int throwCount)
        {
            var value = 0;
            for (int i = 0; i < throwCount; i++)
            {
                var result = StaticRandom.ThrowDice();
                if (result == 1)
                {
                    value = 0;
                    break;
                }
                if (i % 2 == 0) // always the 1st (odd) dice
                {
                    result *= 2;
                }
                value += result;
            }
            return value;
        }
    }
}