using Caliburn.Micro;
using Maintenance.Common.Entities;
using System;
using System.Collections.Generic;

namespace Maintenance.CompositeBenchmarks.ViewModels
{
    public partial class MainViewModel
    {
        private readonly BindableCollection<PortfolioViewModel> portfolios = new BindableCollection<PortfolioViewModel>();
        public BindableCollection<PortfolioViewModel> Portfolios { get { return portfolios; } }

        private readonly BindableCollection<CompositeBenchmarkItemViewModel> compositeBenchmarkItems
            = new BindableCollection<CompositeBenchmarkItemViewModel>();
        public BindableCollection<CompositeBenchmarkItemViewModel> CompositeBenchmarkItems
        { get { return compositeBenchmarkItems; } }

        private readonly Dictionary<long, AssetMap> allAssetMaps = new Dictionary<long, AssetMap>();
        private readonly Dictionary<long, AssetMapComponent> allAssetMapComponents = new Dictionary<long, AssetMapComponent>();
        private readonly Dictionary<long, Index> allIndexes = new Dictionary<long, Index>();
        private readonly Dictionary<long, Portfolio> allPortfolios = new Dictionary<long, Portfolio>();
        private readonly Dictionary<long, PortfolioAssetMapLink> allLinks = new Dictionary<long, PortfolioAssetMapLink>();
        private readonly Dictionary<long, CompositeBenchmarkItem> allBenchmarkComponents = new Dictionary<long, CompositeBenchmarkItem>();


        /// <summary>
        /// Records the asset map component parent-child relationship. Key is child's id, value is parent's id.
        /// </summary>
        private readonly Dictionary<long, long> assetMapComponentParents = new Dictionary<long, long>();
        /// <summary>
        /// Records the trinity using link as id, ptf and asm id as values.
        /// </summary>
        private readonly Dictionary<long, Tuple<long, long>> links = new Dictionary<long, Tuple<long, long>>();

        private bool isOptionsFlyoutOpen;
        public bool IsOptionsFlyoutOpen
        {
            get { return isOptionsFlyoutOpen; }
            set { SetNotify(ref isOptionsFlyoutOpen, value); }
        }

        private bool isMainViewEnabled;
        public bool IsMainViewEnabled
        {
            get { return isMainViewEnabled; }
            set { SetNotify(ref isMainViewEnabled, value); }
        }

        public bool UseDefaultOptions { get; set; }

        public IEditorViewModel Editor { get; set; }
        public IOptionsFlyoutViewModel OptionsFlyout { get; set; }

        public bool IsShowExpiredCompositeBenchmarks { get; set; }
        public bool IsShowNonDefaultCompositeBenchmarks { get; set; }

        public bool IsShowAllBenchmarkComponents { get; set; }

        public bool IsAutoPopulateNewCompositeBenchmark { get; set; }

        private CompositeBenchmarkItemViewModel selectedCompositeBenchmarkItem;
        public CompositeBenchmarkItemViewModel SelectedCompositeBenchmarkItem
        {
            get { return selectedCompositeBenchmarkItem; }
            set { SetNotify(ref selectedCompositeBenchmarkItem, value); }
        }

        private PortfolioAssetMapLinkViewModel selectedPortfolioAssetMapLink;
        public PortfolioAssetMapLinkViewModel SelectedPortfolioAssetMapLink
        {
            get { return selectedPortfolioAssetMapLink; }
            set { SetNotify(ref selectedPortfolioAssetMapLink, value); }
        }

        private PortfolioViewModel selectedPortfolio;
        public PortfolioViewModel SelectedPortfolio
        {
            get { return selectedPortfolio; }
            set { SetNotify(ref selectedPortfolio, value); }
        }
    }
}