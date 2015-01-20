using System.Collections.Generic;
using System.Linq;

namespace Trading.StrategyBuilder.Core
{
    public static class ConditionManager
    {
        public static List<Filter> AllFilters { get; private set; }
        public static IEnumerable<ConditionResult> AllConditionResults
        {
            get { return AllFilters.Select(f => f.ConditionResult); }
        }
        public static List<ConditionResult> UsedConditionResults { get; private set; }
        public static List<ConditionResult> AvailableConditionResults { get; private set; }

        static ConditionManager()
        {
            AllFilters = new List<Filter>();
            UsedConditionResults = new List<ConditionResult>();
            AvailableConditionResults = new List<ConditionResult>();
        }
    }
}