using Caliburn.Micro;
using Trading.SecurityResearch.ViewModels.Entities;

namespace Trading.SecurityResearch.ViewModels
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