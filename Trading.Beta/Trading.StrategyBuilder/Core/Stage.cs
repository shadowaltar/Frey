using System.Collections.Generic;

namespace Trading.StrategyBuilder.Core
{
    public class Stage
    {
        public string Name { get; set; }
        public StageType Type { get; set; }
        public List<Condition> Conditions { get; private set; }
        public Decision SatisfiedDecision { get; set; }

        public Stage NextStage { get { return SatisfiedDecision == null ? null : SatisfiedDecision.NextStage; } }

        public Stage()
        {
            Conditions = new List<Condition>();
        }

        public Stage(string name, StageType type)
            : this()
        {
            Name = name;
            Type = type;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}