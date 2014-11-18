using System.Collections.Generic;
using System.Threading;

namespace Trading.StrategyBuilder.Core
{
    /// <summary>
    /// Represents the decision of trading after going through a chain of conditions.
    /// </summary>
    public class TradingAction
    {
    }

    /// <summary>
    /// Represents a set of conditions linked together. It contains sequentially linked conditions
    /// which would yield trading decisions at the end of chain.
    /// </summary>
    public class ProcessTree
    {
        public List<Filter> Filters { get; set; }

        private int traversalCounter;

        public bool HasNext()
        {
            return Filters.Count > traversalCounter;
        }

        public Filter NextLevel()
        {
            var i = Interlocked.Increment(ref traversalCounter);
            return Filters[i];
        }

        public void ResetPosition()
        {
            Interlocked.CompareExchange(ref traversalCounter, 0, 0);
        }
    }
}