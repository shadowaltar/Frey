using Trading.Common.Entities;

namespace Trading.TradeWatch.ViewModels.Entities
{
    public class HoldingPerformance
    {
        public Security Security { get; set; } 
        public Price Price { get; set; } 
        public Position Position { get; set; } 
        public string BloombergCode { get; set; }
        public decimal Weight { get; set; }
        public decimal PriceChange { get; set; } 
        public decimal PriceChangePercentage { get; set; } 
        public decimal Volume { get; set; } 
    }
}