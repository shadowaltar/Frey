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
    }
}
