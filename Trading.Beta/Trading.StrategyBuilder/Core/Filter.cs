using System.Collections.Generic;

namespace Trading.StrategyBuilder.Core
{
    public class Filter
    {
        public string DisplayName { get; set; }
        public List<Condition> Conditions { get; set; }
        public ConditionResult ConditionResult { get; set; }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}