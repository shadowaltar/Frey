namespace Trading.StrategyBuilder.Core
{
    [Equals]
    public class Condition
    {
        public string SourceValue { get; set; }
        public string Operator { get; set; }
        public string TargetValue { get; set; }

        public Condition() { }

        public Condition(string sourceValue, string @operator, string targetValue)
        {
            SourceValue = sourceValue;
            Operator = @operator;
            TargetValue = targetValue;
        }
    }
}