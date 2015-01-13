namespace Trading.StrategyBuilder.Core
{
    public class Decision
    {
        public DecisionType Type { get; set; }
        public Stage NextStage { get; set; }

        public override string ToString()
        {
            return string.Format("{0}, NextStage: {1}", Type, NextStage);
        }
    }

    public enum DecisionType
    {
        DoNothing,
        CheckNextStage,
        CloseAllPositions,
        KeepSatisfiedSecurities,
        DiscardSatisfiedSecurities,
        AsLongSignal,
        AsShortSignal,
        AsClosePositionSignal,
        AsAdjustQuantitySignal,
    }
}
