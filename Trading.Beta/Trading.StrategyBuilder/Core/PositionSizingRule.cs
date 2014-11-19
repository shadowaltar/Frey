using System.Collections.Generic;
using Trading.Common.Entities;

namespace Trading.StrategyBuilder.Core
{
    /// <summary>
    /// Represents a set of rules to determine how much to buy/sell the given list of securities
    /// </summary>
    public class PositionSizingRule
    {
        public PositionSizingType Type { get; set; }

        public object Determine(HashSet<Security> filtered, Portfolio currentPortfolio)
        {
            throw new System.NotImplementedException();
        }
    }

    public class PositionSizingType
    {
    }
}