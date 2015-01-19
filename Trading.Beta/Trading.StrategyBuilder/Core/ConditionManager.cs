using System.Collections.Generic;

namespace Trading.StrategyBuilder.Core
{
    public static class ConditionManager
    {
        public static List<Filter> AllFilters { get; private set; }
        public static List<ConditionResult> AllConditionResults { get; private set; }
        public static List<ConditionResult> AvailableConditionResults { get; private set; }

        static ConditionManager()
        {
            AllFilters = new List<Filter>();
            AllConditionResults = new List<ConditionResult>();
            AvailableConditionResults = new List<ConditionResult>();
        }
    }
}