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

    }

    public class PositionSizingType
    {
    }
}