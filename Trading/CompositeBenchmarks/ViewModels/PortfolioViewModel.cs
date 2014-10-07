using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using Maintenance.Common.Entities;
using Maintenance.Common.Entities.Decorators;
using Maintenance.Common.Utils;
using Maintenance.Common.ViewModels;

namespace Maintenance.CompositeBenchmarks.ViewModels
{
    public class PortfolioViewModel : ViewModelBaseSlim, ITreeNode, IHasChildNodes<PortfolioAssetMapLinkViewModel>
    {
        private readonly BindableCollection<PortfolioAssetMapLinkViewModel> children;

        public PortfolioViewModel(Portfolio portfolio, IEnumerable<PortfolioAssetMapLink> links)
        {
            portfolio.ThrowIfNull("portfolio", "Must assign a portfolio into its viewmodel.");
            Portfolio = portfolio;
            var vms = from cb in links
                      orderby cb.EffectiveDate descending, cb.ExpiryDate descending
                      select new PortfolioAssetMapLinkViewModel(cb);
            children = new BindableCollection<PortfolioAssetMapLinkViewModel>(vms);
            IsVisible = true;
        }

        public Portfolio Portfolio { get; private set; }

        public string Code { get { return Portfolio.Code; } }
        public string Name { get { return Portfolio.Name; } }

        public BindableCollection<PortfolioAssetMapLinkViewModel> Children { get { return children; } }

        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set { SetNotify(ref isSelected, value); }
        }

        private bool isExpanded;
        public bool IsExpanded
        {
            get { return isExpanded; }
            set { SetNotify(ref isExpanded, value); }
        }

        private bool isVisible;
        public bool IsVisible
        {
            get { return isVisible; }
            set { SetNotify(ref isVisible, value); }
        }
    }
}