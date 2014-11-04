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
            EnterTime = trade.EnterTime.IsoFormatDateTime();
            ExitTime = trade.ExitTime.IsoFormatDateTime();
            EnterPrice = trade.EnterPrice.ToString("N2");
            ExitPrice = trade.ExitPrice.ToString("N2");
            EnterValue = (trade.EnterPrice * trade.Quantity).ToString("N2");
            ExitValue = (trade.ExitPrice * trade.Quantity).ToString("N2");
            Quantity = trade.Quantity.ToString("N0");
            Return = trade.Return.ToString("N6");
            PnL = trade.PnL.ToString("N6");
        }

        public string SecurityCode { get; set; }
        public string SecurityName { get; set; }
        public string EnterTime { get; set; }
        public string ExitTime { get; set; }
        public string EnterPrice { get; set; }
        public string ExitPrice { get; set; }
        public string EnterValue { get; set; }
        public string ExitValue { get; set; }
        public string Quantity { get; set; }
        public string Return { get; set; }
        public string PnL { get; set; }

        public override string ToString()
        {
            return string.Format("({0}) {1}, [{2}]-[{3}], {4} - {5} x {8}, {6} - {7}, r={9}, pnl={10}",
                SecurityCode, SecurityName, EnterTime, ExitTime, EnterPrice, ExitPrice, EnterValue, ExitValue, Quantity, Return, PnL);
        }
    }
}