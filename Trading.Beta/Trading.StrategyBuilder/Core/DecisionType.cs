namespace Trading.StrategyBuilder.Core
{
    public enum DecisionType
    {
        DoNothing,
        CheckNextStage,
        CloseAllPositions,
        KeepSatisfiedSecurities,
        DiscardSatisfiedSecurities,
        Long,
        Short,
        CloseThePositions,
        AdjustQuantity,
    }
}