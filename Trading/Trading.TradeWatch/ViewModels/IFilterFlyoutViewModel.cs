using Caliburn.Micro;
using Trading.TradeWatch.ViewModels.Entities;

namespace Trading.TradeWatch.ViewModels
{
    public interface IFilterFlyoutViewModel
    {
        bool IsReady { get; set; }
        string Id { get; }
        string Code { get; set; }
        string Name { get; set; }
        string PortfolioManagerName { get; set; }
        string BenchmarkCode { get; set; }
        BindableCollection<MarketViewModel> Markets { get; }
        MarketViewModel SelectedMarket { get; set; }

        void ClearAllFields();
    }
}