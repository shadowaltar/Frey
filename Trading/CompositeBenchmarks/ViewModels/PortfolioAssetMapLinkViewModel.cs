using System;
using System.Collections.Generic;
using System.Linq;
using Maintenance.Common.Entities.Decorators;
using Maintenance.Common.Utils;
using Maintenance.Common.ViewModels;
using Maintenance.Common.Entities;

namespace Maintenance.CompositeBenchmarks.ViewModels
{
    /// <summary>
    /// ViewModel for PortfolioAssetMapLink. It is used as child nodes for portfolios,
    /// but not used as parent nodes for BenchmarkComponents.
    /// </summary>
    public class PortfolioAssetMapLinkViewModel : ViewModelBaseSlim, ITreeNode
    {
        public PortfolioAssetMapLinkViewModel(PortfolioAssetMapLink cb)
        {
            PortfolioAssetMapLink = cb;
            IsVisible = true;
            IsActive = true;
        }

        public PortfolioAssetMapLink PortfolioAssetMapLink { get; set; }

        public string FormattedEffectiveDate { get { return PortfolioAssetMapLink.EffectiveDate.IsoFormat(); } }
        public string FormattedExpiryDate { get { return PortfolioAssetMapLink.ExpiryDate.IsoFormat(); } }
        public bool IsExpired { get { return PortfolioAssetMapLink.IsExpired; } }
        public string AssetMapName { get { return AssetMap == null ? string.Empty : AssetMap.Name; } }

        public Portfolio Portfolio
        {
            get { return PortfolioAssetMapLink.Portfolio; }
            set
            {
                if (PortfolioAssetMapLink.Portfolio == value) return;
                PortfolioAssetMapLink.Portfolio = value;
                Notify();
            }
        }

        public AssetMap AssetMap
        {
            get { return PortfolioAssetMapLink.AssetMap; }
            set
            {
                if (PortfolioAssetMapLink.AssetMap == value) return;
                PortfolioAssetMapLink.AssetMap = value;
                Notify();
            }
        }

        private bool isActive;
        /// <summary>
        /// Get/set the composite benchmark active with respect to a portfolio-asm combination.
        /// </summary>
        public bool IsActive
        {
            get { return isActive; }
            set { SetNotify(ref isActive, value); }
        }

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

        public IEnumerable<CompositeBenchmarkItemViewModel> CreateComponentViewModels()
        {
            PortfolioAssetMapLink.ThrowIfNull("compositeBenchmark",
                "Must provide the PortfolioAssetMapLink to create a list of BenchmarkComponents.");
            return PortfolioAssetMapLink.Components.Select(bc => new CompositeBenchmarkItemViewModel(bc, null));
        }
    }
}