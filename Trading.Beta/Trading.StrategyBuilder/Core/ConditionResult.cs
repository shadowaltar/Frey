namespace Trading.StrategyBuilder.Core
{
    public class ConditionResult
    {
        public string DisplayName { get; set; }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}