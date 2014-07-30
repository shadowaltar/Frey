using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using Maintenance.AssetMaps.Entities;
using Maintenance.Common.Entities;
using Maintenance.Common.Entities.Decorators;
using Maintenance.Common.ViewModels;

namespace Maintenance.AssetMaps.ViewModels
{
    public class PortfolioManagerViewModel : ViewModelBaseSlim, ITreeNode, IHasChildNodes<PortfolioViewModel>
    {
        private readonly PortfolioManager portfolioManager;

        private readonly BindableCollection<PortfolioViewModel> portfolios;
        private bool isExpanded;
        private bool isSelected;
        private bool isVisible;

        public PortfolioManagerViewModel(PortfolioManager portfolioManager, IEnumerable<MapsPortfolio> portfolios)
        {
            this.portfolioManager = portfolioManager;
            IEnumerable<PortfolioViewModel> childrenVMs = from child in portfolios
                select new PortfolioViewModel(child, this);
            this.portfolios = new BindableCollection<PortfolioViewModel>(childrenVMs.ToList());
        }

        public PortfolioManager PortfolioManager
        {
            get { return portfolioManager; }
        }

        public string Name
        {
            get { return portfolioManager.Name; }
        }

        public BindableCollection<PortfolioViewModel> Children
        {
            get { return portfolios; }
        }

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