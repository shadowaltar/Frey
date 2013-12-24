using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
