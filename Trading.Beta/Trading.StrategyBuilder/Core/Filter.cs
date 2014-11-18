namespace Trading.StrategyBuilder.Core
{
    public class Filter
    {
        public DataProcessing Preprocess { get; set; }
        public Condition Condition { get; set; }
        public DataProcessing Postprocess { get; set; }
    }
}