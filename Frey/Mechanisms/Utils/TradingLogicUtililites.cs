using System;
using Automata.Entities;

namespace Automata.Mechanisms.Utils
{
    public static class TradingLogicUtililites
    {
        public static Side Opposite(this Side side)
        {
            switch (side)
            {
                case Side.Short:
                    return Side.Long;
                case Side.Long:
                    return Side.Short;
                default:
                    return Side.Hold;
            }
        }

        public static string Code(this Currency currency)
        {
            return "CURRENCY:" + currency;
        }

        public static string Code(this Forex forex)
        {
            return "FOREX:" + forex.Code;
        }

        public static double ValueOf(this Price price, PriceType priceType)
        {
            switch (priceType)
            {
                case PriceType.Close:
                    return price.Close;
                case PriceType.Open:
                    return price.Open;
                case PriceType.High:
                    return price.High;
                case PriceType.Low:
                    return price.Low;
                case PriceType.Median:
                    return (price.Low + price.High) / 2;
                case PriceType.Typical:
                    return (price.Low + price.High + price.Close) / 3;
                case PriceType.WeightedClose:
                    return (price.Low + price.High + price.Close + price.Close) / 4;
                default:
                    throw new ArgumentException();
            }
        }
    }
}
