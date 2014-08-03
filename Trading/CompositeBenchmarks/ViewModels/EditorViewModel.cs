using Caliburn.Micro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Maintenance.Common.Data;
using Maintenance.Common.Entities;
using Maintenance.Common.Utils;
using Maintenance.Common.ViewModels;
using Maintenance.CompositeBenchmarks.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;

namespace Maintenance.CompositeBenchmarks.ViewModels
{
    public partial class EditorViewModel : ViewModelBase, IEditorViewModel
    {
        public EditorViewModel(IViewService viewService)
        {
            AssetMaps = new BindableCollection<AssetMapViewModel>();
            Indexes = new BindableCollection<Index>();
            Portfolios = new BindableCollection<Portfolio>();
            AllPortfolioAssetMapLinks = new List<PortfolioAssetMapLink>();

            ViewService = viewService;

            BenchmarkComponents = new BindingList<CompositeBenchmarkItemViewModel>();
            Mode = EditorMode.Add;

            DisplayName = "Add a new Composite Benchmark";
        }

        /// <summary>
        /// Flag that disable(ignore)/enable the event handler invoked by any property changes
        /// inside the cb component tree structure.
        /// </summary>
        private bool ignoreBenchmarkComponentChangeEvents = true;

        public BindingList<CompositeBenchmarkItemViewModel> BenchmarkComponents { get; private set; }
        public BindableCollection<AssetMapViewModel> AssetMaps { get; private set; }
        public BindableCollection<Index> Indexes { get; private set; }
        public BindableCollection<Portfolio> Portfolios { get; private set; }

        /// <summary>
        /// All cbs.
        /// </summary>
        public List<PortfolioAssetMapLink> AllPortfolioAssetMapLinks { get; private set; }

        public IViewService ViewService { get; set; }
        public IDataAccessFactory<CompositeBenchmarkDataAccess> DataAccessFactory { get; set; }

        private PortfolioAssetMapLink initialPortfolioAssetMapLink;
        private Portfolio currentPortfolio;

        private PortfolioAssetMapLink portfolioAssetMapLink;

        public Dictionary<long, AssetMapComponent> AllComponents { get; set; }
        public bool IsReady { get; set; }

        public EditorMode Mode { get; private set; }

        private Portfolio selectedPortfolio;
        public Portfolio SelectedPortfolio
        {
            get { return selectedPortfolio; }
            set
            {
                if (SetNotify(ref selectedPortfolio, value))
                {
                    // selecting a ptf will affects current selected effective date.
                    InitializeDates(value, SelectedAssetMap == null ? null : SelectedAssetMap.AssetMap);
                    if (value != null)
                    {
                        CanSave = SelectedPortfolio != null && SelectedAssetMap != null;
                    }
                }
            }
        }

        private AssetMapViewModel selectedAssetMap;
        public AssetMapViewModel SelectedAssetMap
        {
            get { return selectedAssetMap; }
            set
            {
                if (SetNotify(ref selectedAssetMap, value) && value != null)
                {
                    CanSave = SelectedPortfolio != null && SelectedAssetMap != null;

                    ignoreBenchmarkComponentChangeEvents = true;
                    ConstructBenchmarkComponentTrees(value.AssetMap);
                    ignoreBenchmarkComponentChangeEvents = false;

                    CheckAllComponentAggregation();
                    CalculateWeights();
                }
            }
        }

        private DateTime effectiveDate;
        public DateTime EffectiveDate
        {
            get { return effectiveDate; }
            set { SetNotify(ref effectiveDate, value); }
        }

        private DateTime expiryDate;
        public DateTime ExpiryDate
        {
            get { return expiryDate; }
            set { SetNotify(ref expiryDate, value); }
        }

        private bool canSave;
        public bool CanSave
        {
            get { return canSave; }
            set { SetNotify(ref canSave, value); }
        }

        private bool isEffectiveDateEnabled;
        public bool IsEffectiveDateEnabled
        {
            get { return isEffectiveDateEnabled; }
            set { SetNotify(ref isEffectiveDateEnabled, value); }
        }

        private bool isPortfoliosEnabled;
        public bool IsPortfoliosEnabled
        {
            get { return isPortfoliosEnabled; }
            set { SetNotify(ref isPortfoliosEnabled, value); }
        }

        private bool isAssetMapsEnabled;
        public bool IsAssetMapsEnabled
        {
            get { return isAssetMapsEnabled; }
            set { SetNotify(ref isAssetMapsEnabled, value); }
        }

        private decimal totalWeight;
        public decimal TotalWeight
        {
            get { return totalWeight; }
            set { SetNotify(ref totalWeight, value); }
        }

        private PortfolioAssetMapLink lastEffectivePortfolioAssetMapLink;

        /// <summary>
        /// Close the view; it will also deregister all event handlers.
        /// </summary>
        /// <param name="callback"></param>
        public override void CanClose(Action<bool> callback)
        {
            base.CanClose(callback);

            // unbind listener to monitor properties inside items changed events
            BenchmarkComponents.ListChanged -= BenchmarkComponents_ListChanged;
            foreach (var bcvm in BenchmarkComponents)
            {
                bcvm.UnbindListChangedForChildren(BenchmarkComponents_ListChanged);
            }
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            ViewService.Window = (MetroWindow)view;
        }

        /// <summary>
        /// Save your changes (ptf, asm, cb). Called by UI.
        /// </summary>
        public async void Save()
        {
            // UI element visual error check
            if (ViewService.HasError<TextBox>() || ViewService.HasError<ComboBox>())
            {
                await ViewService.ShowError("Please correct the error(s) in the fields before saving.");
                return;
            }

            // benchmark and invalid weight check
            foreach (var bc in BenchmarkComponents.Flatten(bc => bc.Children))
            {
                // in xaml if a non-numeric string is given to the weight, it will be fallen back to -1
                if (bc.Weight < 0 || bc.Weight > 100)
                {
                    await ViewService.ShowError(string.Format("For Asset Map component \"{0}\", " +
                                                              "you must assign a valid weight."
                                                              , bc.AssetMapComponent.Name));
                    return;
                }

                // if a weight is > 0 && < 100 but no benchmark is assigned
                if (!bc.IsAggregated && bc.Weight > 0 && bc.Index == null)
                {
                    await ViewService.ShowError(string.Format("For Asset Map component \"{0}\", you must also assign " +
                        "a benchmark to it.", bc.AssetMapComponent.Name));
                    return;
                }

                // if a node is aggregated, no children allows to have bmk or weight
                if (bc.IsAggregated && bc.Index != null)
                {
                    await ViewService.ShowError(string.Format("Benchmark Target assigned at \"{0}\". It is not possible to " +
                        "also assign a target to a component belonging to \"{0}\".",
                        bc.AssetMapComponent.Name));
                    return;
                }

                // if a node has benchmark but no weight
                if (bc.Weight <= 0 && bc.Index != null)
                {
                    await ViewService.ShowError(string.Format("For Asset Map component \"{0}\", please input a weight " +
                        "if a benchmark is assigned.", bc.AssetMapComponent.Name));
                    return;
                }
            }
            // total weight check
            if (TotalWeight != 100)
            {
                await ViewService.ShowError("The total benchmark target weight should be equal to 100%.");
                return;
            }

            // glue up the link
            portfolioAssetMapLink.AssetMap = SelectedAssetMap.AssetMap;
            portfolioAssetMapLink.Portfolio = SelectedPortfolio;

            portfolioAssetMapLink.EffectiveDate = EffectiveDate;
            portfolioAssetMapLink.ExpiryDate = ExpiryDate;

            var decision = await ViewService.AskSave();
            if (decision != MessageDialogResult.Affirmative)
                return;

            // now save
            var saveResult = SaveResult.Unknown;
            try
            {
                if (Mode == EditorMode.Edit)
                {
                    saveResult = await InternalSaveEdit();
                }
                else
                {
                    saveResult = await InternalSaveNew();
                }
            }
            catch (Exception e)
            {
                Log.Error("Error occurs when saving data to database.", e);
            }

            // after save
            if (saveResult == SaveResult.Success)
            {
                // disable save button if success
                CanSave = false;

                switch (Mode)
                {
                    case EditorMode.Edit:
                        await ViewService.ShowMessage("Entry updated",
                            string.Format("The modifications of composite benchmark for portfolio \"{0}\" and asset map \"{1}\"" +
                                          " is saved successfully.", portfolioAssetMapLink.Portfolio.Code,
                                          portfolioAssetMapLink.AssetMap.Code));
                        Publish<IMainViewModel>(ActivityType.Edit, "PortfolioAssetMapLink");
                        break;
                    default: // case EditorMode.Add
                        await ViewService.ShowMessage("New entry created",
                            string.Format("The new composite benchmark for portfolio \"{0}\" and asset map \"{1}\"" +
                                          " is created successfully.", portfolioAssetMapLink.Portfolio.Code,
                                          portfolioAssetMapLink.AssetMap.Code));
                        Publish<IMainViewModel>(ActivityType.Add, "PortfolioAssetMapLink");
                        break;
                }

                // close the dialog
                TryClose(true);
            }
            else
            {
                string message;
                switch (saveResult)
                {
                    case SaveResult.CannotAddEntry:
                        message = string.Format(
@"Cannot create composite benchmark effective on {0}.", EffectiveDate.IsoFormat());
                        break;
                    case SaveResult.CannotAddComponentEntry:
                        message = string.Format(
@"Cannot create benchmark items in the new composite benchmark effective on {0}.", EffectiveDate.IsoFormat());
                        break;
                    case SaveResult.CannotModifyComponentEntry:
                        message = string.Format(
@"Cannot modify the indexes or weights for the composite benchmark effective on {0}.", EffectiveDate.IsoFormat());
                        break;
                    case SaveResult.ExpirySmallerThanEffective:
                        message = string.Format(
@"For a link between portfolio and asset map, you must specify an effective date which is larger than the date of a previously link ""{0}""."
                            , lastEffectivePortfolioAssetMapLink.EffectiveDate.IsoFormat());
                        break;
                    case SaveResult.CannotModifyLastEntry:
                        message =
@"Cannot adjust the expiry date of the previous link between portfolio and asset map.";
                        break;
                    default: // this will never happen.
                        message =
@"An error has occurred preventing data from being saved. If this problem persists, please contact IMS support for further assistance.";
                        break;
                }
                await ViewService.ShowError(message);
            }
        }

        /// <summary>
        /// Clear your modifications and reset everything. Called by UI.
        /// </summary>
        public void Reset()
        {
            SetItem(initialPortfolioAssetMapLink, currentPortfolio);
        }

        /// <summary>
        /// Clear the benchmark and its weight for a cb component. It is called by UI.
        /// </summary>
        /// <param name="component"></param>
        public void ClearBenchmarkComponent(CompositeBenchmarkItemViewModel component)
        {
            component.Weight = 0;
            component.Index = null;
            if (component.IsAggregated)
            {
                foreach (var child in component.Children)
                {
                    ClearBenchmarkComponent(child);
                }
            }
            else
            {
                component.CheckAncestorAggregation();
            }
            // calculate weights after u cleared any component
            CalculateWeights();
        }

        public void InitializeMode(EditorMode mode)
        {
            Mode = mode;
            switch (Mode)
            {
                case EditorMode.Add:
                    DisplayName = "Add a new Portfolio, AssetMap and Composite Benchmark Link";
                    IsPortfoliosEnabled = IsAssetMapsEnabled = IsEffectiveDateEnabled = true;
                    break;
                case EditorMode.Edit:
                    DisplayName = "Edit Portfolio, AssetMap and Composite Benchmark Link";
                    IsPortfoliosEnabled = IsAssetMapsEnabled = IsEffectiveDateEnabled = false;
                    break;
            }
        }

        /// <summary>
        /// Sets an item to the editor dialog. It is called by main UI, or by this UI for a reset.
        /// If the link and ptf are provided, cb item values will be filled in.
        /// </summary>
        /// <param name="link"></param>
        /// <param name="portfolio"></param>
        public void SetItem(PortfolioAssetMapLink link, Portfolio portfolio)
        {
            ignoreBenchmarkComponentChangeEvents = true;

            initialPortfolioAssetMapLink = link;
            currentPortfolio = portfolio;
            BenchmarkComponents.Clear();
            SelectedAssetMap = null; // need to reset to trigger the tree population later
            SelectedPortfolio = null;

            portfolioAssetMapLink = new PortfolioAssetMapLink();
            // if in edit mode, fill in missing info for the link
            if (Mode == EditorMode.Edit)
            {
                portfolioAssetMapLink = new PortfolioAssetMapLink
                {
                    Id = link.Id,
                    Portfolio = link.Portfolio,
                    AssetMap = link.AssetMap,
                    CreateTime = link.CreateTime,
                    Creator = link.Creator,
                    EffectiveDate = link.EffectiveDate,
                    ExpiryDate = link.ExpiryDate,
                    // ignore Components, Updater, UpdateTime
                };
            }

            IList<CompositeBenchmarkItemViewModel> componentsInUi = null;
            if (link != null && portfolio != null)
            {
                var asm = link.AssetMap;
                // use the instance in IObservableCollection instead of the args directly
                SelectedPortfolio = Portfolios.FirstOrDefault(p => p == portfolio);
                SelectedAssetMap = AssetMaps.FirstOrDefault(asmvm => asmvm.AssetMap == asm); // will populate tree
                // "ConstructBenchmarkComponentTrees()" is triggered by above

                var sourceComponents = link.Components.Flatten(c => c.Components);
                componentsInUi = BenchmarkComponents.Flatten(c => c.Children);

                foreach (var bcvm in componentsInUi)
                {
                    var source = sourceComponents.FirstOrDefault(s =>
                        s.AssetMapComponent.Id == bcvm.AssetMapComponent.Id && s.IsReal);
                    if (source != null)
                    {
                        bcvm.Index = source.Index;
                        bcvm.Weight = source.Weight;

                        // its parent is in 'Aggregated' status, and must be expanded.
                        bcvm.CheckAncestorAggregation();
                    }
                }

                // generate predicted dates
                InitializeDates(portfolio, link.AssetMap);
            }
            else if (link == null && portfolio != null)
            {
                SelectedPortfolio = Portfolios.FirstOrDefault(p => p == portfolio);
                InitializeDates(portfolio, null);
            }
            else
            {
                SelectedPortfolio = null;
                SelectedAssetMap = null;
                InitializeDates(null, null);

                BenchmarkComponents.Clear();
                CanSave = false;
            }

            CalculateWeights();
            ignoreBenchmarkComponentChangeEvents = false;

            // expand if aggregated
            if (componentsInUi != null)
            {
                foreach (var bcvm in componentsInUi.Where(c => c.IsAggregated))
                {
                    bcvm.IsExpanded = true;
                }
            }
        }

        /// <summary>
        /// Calculate the total weight. For cb components with IsAggregated=true, it will use children weight
        /// sum.
        /// </summary>
        private void CalculateWeights()
        {
            TotalWeight = BenchmarkComponents.Sum(b => b.IsAggregated ? b.ChildrenWeight : b.Weight);
        }

        /// <summary>
        /// If no ptf is selected or ptf has no previous cb at all, use min date.
        /// Otherwise use last cb's effective date + 1 day.
        /// </summary>
        /// <param name="portfolio"></param>
        /// <param name="assetMap"></param>
        private void InitializeDates(Portfolio portfolio, AssetMap assetMap)
        {
            if (portfolio != null && assetMap != null)
            {
                var lastLink = AllPortfolioAssetMapLinks.Where(cb =>
                    cb.Portfolio == portfolio && cb.AssetMap == assetMap)
                    .OrderByDescending(cb => cb.EffectiveDate).FirstOrDefault();
                EffectiveDate = lastLink != null
                    ? lastLink.EffectiveDate.AddDays(1) : DateTime.Today.Date;
            }
            else
            {
                EffectiveDate = DateTime.Today.Date;
            }

            ExpiryDate = Common.Constants.MaxExpiryDate;
        }

        /// <summary>
        /// When an asm is selected, build the cb tree and represent it to the view.
        /// </summary>
        /// <param name="assetMap"></param>
        private void ConstructBenchmarkComponentTrees(AssetMap assetMap)
        {
            var firstLevelNodes = assetMap.RootComponent.Children;
            RecursiveAddBenchmarkComponent(firstLevelNodes, portfolioAssetMapLink);
            var cbvm = new PortfolioAssetMapLinkViewModel(portfolioAssetMapLink);

            ignoreBenchmarkComponentChangeEvents = true;

            BenchmarkComponents.Clear();
            BenchmarkComponents.ListChanged += BenchmarkComponents_ListChanged;
            foreach (var bcvm in cbvm.CreateComponentViewModels())
            {
                bcvm.CompositeBenchmarkItem.PortfolioAssetMapLink = portfolioAssetMapLink;
                BenchmarkComponents.Add(bcvm);

                bcvm.BindListChangedForChildren(BenchmarkComponents_ListChanged);
            }

            ignoreBenchmarkComponentChangeEvents = false;
        }

        /// <summary>
        /// Recursive method called during cb component tree construction.
        /// </summary>
        /// <param name="asmComps"></param>
        /// <param name="parent"></param>
        private void RecursiveAddBenchmarkComponent(IEnumerable<AssetMapComponent> asmComps,
            IHasChildCompositeBenchmarkItems parent)
        {
            foreach (var assetMapComponent in asmComps)
            {
                // make the placeholder bc item (which is a bc without a bmk)
                var component = CompositeBenchmarkItem.NewPlaceholder(assetMapComponent);
                component.PortfolioAssetMapLink = portfolioAssetMapLink;

                parent.Components.Add(component);
                if (assetMapComponent.Children.Count > 0)
                {
                    RecursiveAddBenchmarkComponent(assetMapComponent.Children, component);
                }
            }
        }

        /// <summary>
        /// Invoked if the cb components properties are changed.
        /// Only monitors property "Weight" and "Benchmark" (others like IsSelected is not handled here).
        /// When they are changed, trigger calculation and checks for weights and IsAggregated statuses.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BenchmarkComponents_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (ignoreBenchmarkComponentChangeEvents)
                return;

            switch (e.ListChangedType)
            {
                case ListChangedType.ItemChanged:
                    {
                        ignoreBenchmarkComponentChangeEvents = true;
                        CheckAllComponentAggregation();
                        ignoreBenchmarkComponentChangeEvents = false;

                        var propertyName = e.PropertyDescriptor.Name;
                        if (propertyName == "Weight")
                        {
                            CalculateWeights();
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// Go through all the cb components and check their 'IsAggregated' status.
        /// This status affects how the weight is calculated for a component: if true, it
        /// uses children weight sum as its weight, and in the view you cannot select benchmark + weight;
        /// if false, it uses its own weight.
        /// </summary>
        private void CheckAllComponentAggregation()
        {
            foreach (var bcvm in BenchmarkComponents.Flatten(bc => bc.Children))
            {
                bcvm.CheckAncestorAggregation();
            }
        }
    }
}