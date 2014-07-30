using Caliburn.Micro;
using Maintenance.Common.Entities;
using Maintenance.Common.Utils;
using Maintenance.Portfolios.Entities;
using Portfolio = Maintenance.Portfolios.Entities.Portfolio;

namespace Maintenance.Portfolios.ViewModels
{
    public interface IEditFlyoutViewModel : IHasViewService, IHasDataAccessFactory<PortfolioDataAccess>
    {
        BindableCollection<string> SectorSchemes { get; }
        BindableCollection<string> AssetClassFocuses { get; }
        BindableCollection<Location> Locations { get; }
        bool IsReady { get; set; }
        bool CanSave { get; set; }
        void SetItem(Portfolio selectedPortfolio, PortfolioExtendedInfo selectedInfo);
    }
}