using Trading.Common.Entities;

namespace Trading.Backtest.Reporting
{
    public class PriceReportEntry : Price
    {
        public double Return { get; set; }
    }
}