using System;

namespace Trading.Backtest.Reporting
{
    public class PortfolioStatus
    {
        public PortfolioStatus(DateTime time, double equity)
        {
            Time = time;
            Equity = equity;
        }

        public DateTime Time { get; set; }
        public double Equity { get; set; }
    }
}