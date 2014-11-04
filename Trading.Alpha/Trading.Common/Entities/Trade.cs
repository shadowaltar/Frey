using System;

namespace Trading.Common.Entities
{
    public class Trade
    {
        public long Id { get; set; }
        public Security Security { get; set; }

        public DateTime EnterTime { get; set; }
        public DateTime ExitTime { get; set; }
        public TimeSpan HoldingPeriod { get { return ExitTime - EnterTime; } }

        public double EnterPrice { get; set; }
        public double ExitPrice { get; set; }
        public double Return { get { return (ExitPrice - EnterPrice) / EnterPrice; } }
        public double LogReturn { get { return Math.Log(ExitPrice / EnterPrice); } }
        public double Quantity { get; set; }
        public double Value { get { return ExitPrice * Quantity; } }
        public double PnL { get { return (ExitPrice - EnterPrice) * Quantity; } }

        public ExitType ExitType { get; set; }
    }
}