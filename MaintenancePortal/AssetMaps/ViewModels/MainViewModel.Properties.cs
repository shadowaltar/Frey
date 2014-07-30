using Caliburn.Micro;
using Maintenance.AssetMaps.Entities;
using Maintenance.Common.Entities;
using System.Collections.Generic;

namespace Maintenance.AssetMaps.ViewModels
{
    public partial class MainViewModel
    {
        private readonly BindableCollection<ComponentPropertyViewModel> componentProperties =
            new BindableCollection<ComponentPropertyViewModel>();

        private readonly BindableCollection<AssetMapComponentViewModel> assetMapComponents =
            new BindableCollection<AssetMapComponentViewModel>();

        private readonly BindableCollection<PortfolioManagerViewModel> portfolioManagers =
            new BindableCollection<PortfolioManagerViewModel>();

        private readonly BindableCollection<AssetMap> uiAssetMaps =
            new BindableCollection<AssetMap>();

        private AssetMapComponentViewModel selectedComponent;
        private MapsPortfolio selectedPortfolio;
        private PortfolioManager selectedPortfolioManager;
        private AssetMap selectedAssetMap;

        private Dictionary<long, AssetMapComponent> allComponents;
        private Dictionary<long, AssetMapComponentProperty> allProperties;
        private Dictionary<long, long> componentToAsms;
        private Dictionary<long, long> propertyToComponents;
        private Dictionary<long, AssetMap> allAssetMaps;
        private Dictionary<long, MapsPortfolio> allPortfolios;
        private Dictionary<long, PortfolioAssetMapLink> allLinks;
        private HashSet<long> nonDeletableAsmComponentIds;

        private bool canEdit;

        private bool isAddFlyoutOpen;
        private bool isEditFlyoutOpen;
        private bool isFilterFlyoutOpen;

        private bool isLoadingNodes;
        private bool isModified;
        private bool isOptionsFlyoutOpen;
        private bool isPortfolioListVisible;
        private bool isAssetMapListVisible;

        public AssetMapComponentViewModel SelectedComponent
        {
            get { return selectedComponent; }
            set { SetNotify(ref selectedComponent, value); }
        }

        public MapsPortfolio SelectedPortfolio
        {
            get { return selectedPortfolio; }
            set { SetNotify(ref selectedPortfolio, value); }
        }

        public PortfolioManager SelectedPortfolioManager
        {
            get { return selectedPortfolioManager; }
            set { SetNotify(ref selectedPortfolioManager, value); }
        }

        public AssetMap SelectedAssetMap
        {
            get { return selectedAssetMap; }
            set
            {
                if (SetNotify(ref selectedAssetMap, value) && value != null)
                {
                    if (AssetMapComponents.Count == 0 || AssetMapComponents[0].Component.AsmId != value.Id)
                    {
                        InitializeAssetMapStructure();
                    }
                }
                if (IsAssetMapListVisible)
                {
                    CanEdit = value != null;
                }
            }
        }

        public BindableCollection<AssetMapComponentViewModel> AssetMapComponents
        {
            get { return assetMapComponents; }
        }

        public BindableCollection<ComponentPropertyViewModel> ComponentProperties
        {
            get { return componentProperties; }
        }

        public BindableCollection<PortfolioManagerViewModel> PortfolioManagers
        {
            get { return portfolioManagers; }
        }

        public BindableCollection<AssetMap> AssetMaps
        {
            get { return uiAssetMaps; }
        }

        public IOptionsFlyoutViewModel OptionsFlyout { get; private set; }
        public IEditorViewModel Editor { get; set; }

        public bool IsLoadingNodes
        {
            get { return isLoadingNodes; }
            set { SetNotify(ref isLoadingNodes, value); }
        }

        public bool IsAddFlyoutOpen
        {
            get { return isAddFlyoutOpen; }
            set { SetNotify(ref isAddFlyoutOpen, value); }
        }

        public bool IsEditFlyoutOpen
        {
            get { return isEditFlyoutOpen; }
            set { SetNotify(ref isEditFlyoutOpen, value); }
        }

        public bool IsFilterFlyoutOpen
        {
            get { return isFilterFlyoutOpen; }
            set { SetNotify(ref isFilterFlyoutOpen, value); }
        }

        public bool IsOptionsFlyoutOpen
        {
            get { return isOptionsFlyoutOpen; }
            set { SetNotify(ref isOptionsFlyoutOpen, value); }
        }

        public bool CanEdit
        {
            get { return canEdit; }
            set { SetNotify(ref canEdit, value); }
        }

        public bool IsModified
        {
            get { return isModified; }
            set { SetNotify(ref isModified, value); }
        }

        public bool IsPortfolioListVisible
        {
            get { return isPortfolioListVisible; }
            set
            {
                if (SetNotify(ref isPortfolioListVisible, value))
                {
                    IsAssetMapListVisible = !value;
                    SelectedPortfolio = null;
                    SelectedPortfolioManager = null;
                }
            }
        }

        /// <summary>
        /// Not to be set directly, but mean to be triggered by <see cref="IsPortfolioListVisible"/>.
        /// </summary>
        public bool IsAssetMapListVisible
        {
            get { return isAssetMapListVisible; }
            set { SetNotify(ref isAssetMapListVisible, value); }
        }
    }
}