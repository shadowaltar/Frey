using Caliburn.Micro;
using Maintenance.AssetMaps.Entities;
using Maintenance.Common.Entities;
using Maintenance.Common.Entities.Decorators;
using Maintenance.Common.ViewModels;

namespace Maintenance.AssetMaps.ViewModels
{
    public class PortfolioViewModel : ViewModelBaseSlim, ITreeNode, IHasChildNodes<object>
    {
        public PortfolioViewModel(MapsPortfolio portfolio, PortfolioManagerViewModel portfolioManager)
        {
            this.portfolio = portfolio;
            this.portfolioManager = portfolioManager;
            Children = null; // it doesn't have any child.
        }

        private readonly MapsPortfolio portfolio;
        private readonly PortfolioManagerViewModel portfolioManager;
        private bool isExpanded;
        private bool isSelected;
        private bool isVisible;

        public MapsPortfolio Portfolio { get { return portfolio; } }
        public string Name { get { return portfolio.DisplayName; } }
        public string PortfolioManagerName { get { return portfolioManager.Name; } }

        public BindableCollection<object> Children { get; private set; }

        public bool IsSelected
        {
            get { return isSelected; }
            set { SetNotify(ref isSelected, value); }
        }

        public bool IsExpanded
        {
            get { return isExpanded; }
            set { SetNotify(ref isExpanded, value); }
        }

        public bool IsVisible
        {
            get { return isVisible; }
            set { SetNotify(ref isVisible, value); }
        }
    }
}