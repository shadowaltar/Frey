using System.Collections.Generic;

namespace Trading.StrategyBuilder.Core
{
    public class Stage
    {
        public string Name { get; set; }
        public List<Condition> Conditions { get; private set; }
        public Decision SatisfiedDecision { get; set; }
        public Decision UnsatisfiedDecision { get; set; }

        public Stage()
        {
            Conditions = new List<Condition>();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}