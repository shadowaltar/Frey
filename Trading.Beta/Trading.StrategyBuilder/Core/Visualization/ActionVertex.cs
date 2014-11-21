namespace Trading.StrategyBuilder.Core.Visualization
{
    public class ActionVertex
    {
        public string Formula { get; set; }
        public string Action { get; set; }

        public override string ToString()
        {
            return "IF " + Formula + " THEN " + Action;
        }
    }
}