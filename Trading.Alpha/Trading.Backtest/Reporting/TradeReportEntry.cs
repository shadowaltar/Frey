using Trading.Common.Entities;
using Trading.Common.Utils;

namespace Trading.Backtest.Reporting
{
    public class TradeReportEntry
    {
        public TradeReportEntry(Trade trade)
        {
            SecurityCode = trade.Security.Code;
            SecurityName = trade.Security.Name;
            EnterTime = trade.EnterTime.ToDateInt();
            ExitTime = trade.ExitTime.ToDateInt();
            Parameter = trade.Parameter;
            EnterPrice = trade.EnterPrice;
            ExitPrice = trade.ExitPrice;
            EnterValue = trade.EnterPrice * trade.Quantity;
            ExitValue = trade.ExitPrice * trade.Quantity;
            Quantity = trade.Quantity;
            Return = trade.Return;
            PnL = trade.PnL;
        }

        public string SecurityCode { get; set; }
        public string SecurityName { get; set; }
        public int EnterTime { get; set; }
        public int ExitTime { get; set; }
        public double Parameter { get; set; }
        public double EnterPrice { get; set; }
        public double ExitPrice { get; set; }
        public double EnterValue { get; set; }
        public double ExitValue { get; set; }
        public double Quantity { get; set; }
        public double Return { get; set; }
        public double PnL { get; set; }

        public override string ToString()
        {
            return string.Format("({0}) {1}, [{2}]-[{3}], {4} - {5} x {8}, {6} - {7}, r={9}, pnl={10}",
                SecurityCode, SecurityName, EnterTime, ExitTime, EnterPrice, ExitPrice, EnterValue, ExitValue, Quantity, Return, PnL);
        }
    }
}