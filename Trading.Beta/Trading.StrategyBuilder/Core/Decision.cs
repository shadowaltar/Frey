using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trading.StrategyBuilder.Core
{
    public class Decision
    {

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
