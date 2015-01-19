namespace Trading.StrategyBuilder.Core
{
    public class Decision
    {
        public string Name { get; set; }
        public DecisionType Type { get; set; }
        public DecisionTargetType TargetType { get; set; }
        public Filter Next { get; set; }

        public override string ToString()
        {
            return string.Format("Decision \"{0}\". Action {1} is applied to {2}", Name, Type, TargetType);
        }
    }
}
