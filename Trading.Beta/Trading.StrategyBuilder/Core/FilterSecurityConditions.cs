using System.Collections.Generic;

namespace Trading.StrategyBuilder.Core
{
    public class FilterSecurityConditions : List<Condition>, IConditions
    {
        public FilterSecurityConditions()
        {
        }

        public void AddCondition(string sourceValue, string @operator, string targetValue)
        {
            Add(new Condition(sourceValue, @operator, targetValue));
        }

        public void RemoveCondition(Condition condition)
        {
            Remove(condition);
        }
    }
}