using System;
using System.Collections.Generic;

namespace Trading.StrategyBuilder.Core
{
    [Obsolete]
    public class FilterSecurityConditions : List<Condition>, IConditions
    {
        public FilterSecurityConditions()
        {
        }

        public void AddCondition(string sourceValue, string @operator, string targetValue)
        {
            Add(new Condition(sourceValue, @operator.FromSymbol(), targetValue));
        }

        public void RemoveCondition(Condition condition)
        {
            Remove(condition);
        }
    }
}