using Caliburn.Micro;
using Trading.Common.Entities;
using Trading.Common.Utils;
using Trading.CountryOverrides.Entities;

namespace Trading.CountryOverrides.ViewModels
{
    public interface IEditFlyoutViewModel : IHasViewService, IHasDataAccessFactory<CountryOverrideDataAccess>
    {
        bool IsReady { get; set; }
        bool CanSave { get; set; }
        bool ShowBelongsToPortfolioManager { get; set; }
        bool ShowBelongsToPortfolio { get; set; }
        bool ShowBelongsTo { get; set; }
        OverrideType OverrideType { get; set; }
        Country SelectedCountry { get; set; }
        Portfolio SelectedPortfolio { get; set; }
        PortfolioManager SelectedPortfolioManager { get; set; }

        BindableCollection<Country> Countries { get; }
        BindableCollection<Portfolio> Portfolios { get; }
        BindableCollection<PortfolioManager> PortfolioManagers { get; }

        void SetItem(OverrideItem item);
        void Save();
    }
}