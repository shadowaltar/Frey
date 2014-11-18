using System.Collections.Generic;
using Trading.Common.Entities;

namespace Trading.StrategyBuilder.Core
{
    /// <summary>
    /// Represents a decision flow which results in buying, selling and other trading action
    /// based on a specific schedule (when to trade) and a set of specific inter-linked decisions
    /// (what/which to trade).
    /// </summary>
    public class TradeTrigger
    {
        public TriggerTime TriggerTime { get; set; }

        public ProcessTree Filters { get; set; }

        public PositionSizingRule PositionSizing { get; set; }

        public void Evaluate(HashSet<Security> securities, Portfolio currentPortfolio)
        {
            if (!TriggerTime.IsGood())
                return;

            //  Check all the filters
            HashSet<Security> filtered = null;
            while (Filters.HasNext())
                filtered = Filter(Filters.NextLevel(), filtered);

            // now got the set of securities to trade
            var positionChanges = PositionSizing.Determine(filtered, currentPortfolio);
        }

        private HashSet<Security> Filter(Filter filter, HashSet<Security> targetSecurities)
        {
            throw new System.NotImplementedException();
        }
    }

    public class TradeTriggers
    {
        public List<TradeTrigger> Triggers { get; set; }
    }
}