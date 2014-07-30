using Caliburn.Micro;
using Maintenance.Common.Entities;
using Maintenance.Common.Utils;
using Portfolio = Maintenance.Portfolios.Entities.Portfolio;

namespace Maintenance.Portfolios.ViewModels
{
    public interface IBenchmarkAssociationFlyoutViewModel : IHasViewService, IHasDataAccessFactory<PortfolioDataAccess>
    {
        bool IsReady { get; set; }
        bool CanSave { get; set; }

        Index SelectedIndex { get; set; }

        BindableCollection<Index> Indexes { get; }

        void SetItem(Portfolio item);
        void Save();
    }
}