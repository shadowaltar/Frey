using System;

namespace Trading.Backtest.Reporting
{
    public class PortfolioStatus
    {
        public PortfolioStatus(DateTime time, double equity, double benchmark)
        {
            Time = time;
            Equity = equity;
            Benchmark = benchmark;
        }

        public DateTime Time { get; set; }
        public double Equity { get; set; }
        public double Benchmark { get; set; }
    }
}