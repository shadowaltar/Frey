using Caliburn.Micro;
using Maintenance.Common.Data;
using Maintenance.Common.Entities.Decorators;
using Maintenance.Common.SharedSettings;
using Maintenance.Common.Utils;
using Maintenance.Common.ViewModels;
using Maintenance.CompositeBenchmarks.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Input;

namespace Maintenance.CompositeBenchmarks.ViewModels
{
    public partial class MainViewModel : MainViewModelBase<CompositeBenchmarkDataAccess, ImapSettings>,
        IMainViewModel, IHandle<ActivityMessage<IMainViewModel>>
    {
        public override sealed string ProgramName { get { return "Composite Benchmark Maintenance"; } }

        public MainViewModel(IDataAccessFactory<CompositeBenchmarkDataAccess> dataAccessFactory,
            IEditorViewModel editor, IOptionsFlyoutViewModel optionsFlyout,
            ImapSettings settings)
            : base(dataAccessFactory, settings)
        {
            OptionsFlyout = optionsFlyout;

            Editor = editor;
            Editor.DataAccessFactory = DataAccessFactory;

            UseDefaultOptions = true; // todo add config save/load

            if (UseDefaultOptions)
            {
                IsAutoPopulateNewCompositeBenchmark = true;
                IsShowAllBenchmarkComponents = true;
                IsShowExpiredCompositeBenchmarks = false;
                IsShowNonDefaultCompositeBenchmarks = true;
            }
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            Load();
        }

        public override void CanClose(Action<bool> callback)
        {
            base.CanClose(callback);
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public void SelectPortfolioOrCompositeBenchmark(ITreeNode node)
        {
            var pvm = node as PortfolioViewModel;
            if (pvm != null)
            {
                SelectedPortfolio = pvm;
                SelectedPortfolioAssetMapLink = null;
                CompositeBenchmarkItems.Clear();
            }
            else
            {
                var cbvm = node as PortfolioAssetMapLinkViewModel;
                if (cbvm != null)
                {
                    SelectedPortfolioAssetMapLink = cbvm;
                    SelectedPortfolio = Portfolios.FirstOrDefault(p => p.Portfolio == cbvm.Portfolio);

                    var bcvms = cbvm.CreateComponentViewModels().ToList();
                    CompositeBenchmarkItems.ClearAndAddRange(bcvms);
                    CheckParentAggregated(bcvms);

                    ToggleShowBenchmarkComponents();
                }
            }

            SelectedCompositeBenchmarkItem = null;
        }

        /// <summary>
        /// Check all the benchmark component vms, 
        /// </summary>
        /// <param name="viewModels"></param>
        private static void CheckParentAggregated(IEnumerable<CompositeBenchmarkItemViewModel> viewModels)
        {
            var allComps = viewModels.Flatten(bc => bc.Children);
            foreach (var bcvm in allComps)
            {
                bcvm.CheckAncestorAggregation();
                if (bcvm.Parent != null)
                {
                    bcvm.Parent.IsExpanded = bcvm.Parent.IsAggregated;
                }
            }
        }

        public void SelectBenchmarkComponent(ITreeNode node)
        {
            var bcvm = node as CompositeBenchmarkItemViewModel;
            if (bcvm != null)
            {
                SelectedCompositeBenchmarkItem = bcvm;
            }
        }

        public async void Add()
        {
            PrepareEditor(EditorMode.Add);
            if (IsAutoPopulateNewCompositeBenchmark)
            {
                Editor.SetItem(SelectedPortfolioAssetMapLink == null
                    ? null : SelectedPortfolioAssetMapLink.PortfolioAssetMapLink,
                    SelectedPortfolio == null ? null : SelectedPortfolio.Portfolio);
            }
            else
            {
                Editor.SetItem(null, null);
            }
            await ViewService.ShowDialog(Editor as ViewModelBase);
        }

        public async void Edit()
        {
            if (SelectedPortfolioAssetMapLink == null || SelectedPortfolio == null)
                return;
            PrepareEditor(EditorMode.Edit);
            Editor.SetItem(SelectedPortfolioAssetMapLink.PortfolioAssetMapLink, SelectedPortfolio.Portfolio);
            await ViewService.ShowDialog(Editor as ViewModelBase);
        }

        public new void Refresh()
        {
            Load();
        }

        public void ToggleOptions()
        {
            IsOptionsFlyoutOpen = !IsOptionsFlyoutOpen;
            OptionsFlyout.SelectedEnvironment = environment;
            if (UseDefaultOptions)
            {
                OptionsFlyout.IsPublishingPreference = false;
                OptionsFlyout.IsAutoPopulateNewCompositeBenchmark = IsAutoPopulateNewCompositeBenchmark;
                OptionsFlyout.IsShowAllBenchmarkComponents = IsShowAllBenchmarkComponents;
                OptionsFlyout.IsShowExpiredCompositeBenchmarks = IsShowExpiredCompositeBenchmarks;
                OptionsFlyout.IsShowInactiveCompositeBenchmarks = IsShowNonDefaultCompositeBenchmarks;
                OptionsFlyout.IsPublishingPreference = true;
            }
        }

        public void ExpandAllCompositeBenchmarks()
        {
            foreach (var pvm in Portfolios)
            {
                pvm.IsExpanded = true;
            }
        }

        public void CollapseAllCompositeBenchmarks()
        {
            foreach (var pvm in Portfolios)
            {
                pvm.IsExpanded = false;
            }
        }

        public void ExpandAllCompositeBenchmarkComponents()
        {
            ToggleAllCompositeBenchmarkComponentsExpanded(true, CompositeBenchmarkItems);
        }

        public void CollapseAllCompositeBenchmarkComponents()
        {
            ToggleAllCompositeBenchmarkComponentsExpanded(false, CompositeBenchmarkItems);
        }

        private void ToggleAllCompositeBenchmarkComponentsExpanded(bool isExpanded, IEnumerable<CompositeBenchmarkItemViewModel> targets)
        {
            foreach (var bcvm in targets)
            {
                bcvm.IsExpanded = isExpanded;
                if (!bcvm.Children.IsNullOrEmpty())
                {
                    ToggleAllCompositeBenchmarkComponentsExpanded(isExpanded, bcvm.Children);
                }
            }
        }

        public void About()
        {
            ViewService.ShowMessage("About " + ProgramName,
                "Version: " + Assembly.GetExecutingAssembly().GetName().Version
                + System.Environment.NewLine
                + "Database: " + Settings.GetOracleConnectionServiceName(environment));
        }

        public void HandleDoubleClick(object focusedItem)
        {
            var pamlvm = focusedItem as PortfolioAssetMapLinkViewModel;
            if (pamlvm != null)
            {
                Edit();
            }
        }

        public void HandleShortcutKeys(KeyEventArgs e)
        {
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                switch (e.Key)
                {
                    case Key.N: Add(); break;
                    case Key.E: Edit(); break;
                    case Key.R: Refresh(); break;
                    case Key.O: ToggleOptions(); break;
                }
            }
            if (e.Key == Key.Escape)
            {
                if (IsOptionsFlyoutOpen) ToggleOptions();
            }
        }

        public void Handle(ActivityMessage<IMainViewModel> message)
        {
            switch (message.Type)
            {
                case ActivityType.Add:
                case ActivityType.Edit:
                    if (IsOptionsFlyoutOpen) ToggleOptions();
                    Refresh();
                    break;
                case ActivityType.ChangeEnvironment:
                    if (message.Item != null)
                    {
                        if (DataAccessFactory.Environment != (string)message.Item)
                        {
                            Environment = (string)message.Item;
                            Refresh();
                        }
                    }
                    if (message.Item == null || IsOptionsFlyoutOpen)
                    {
                        ToggleOptions();
                    }
                    break;
                case ActivityType.ChangePreference:
                    var pref = (KeyValuePair<string, bool>)message.Item;
                    switch (pref.Key)
                    {
                        case "IsShowExpiredCompositeBenchmarks":
                            IsShowExpiredCompositeBenchmarks = pref.Value;
                            ToggleShowCompositeBenchmarks();
                            break;
                        case "IsShowNonDefaultCompositeBenchmarks":
                            IsShowNonDefaultCompositeBenchmarks = pref.Value;
                            ToggleShowCompositeBenchmarks();
                            break;
                        case "IsShowAllBenchmarkComponents":
                            IsShowAllBenchmarkComponents = pref.Value;
                            ToggleShowBenchmarkComponents();
                            break;
                        case "IsAutoPopulateNewCompositeBenchmark":
                            IsAutoPopulateNewCompositeBenchmark = pref.Value;
                            break;
                    }
                    break;
            }
        }

        private void PrepareEditor(EditorMode mode)
        {
            if (!Editor.IsReady)
            {
                Editor.AllPortfolioAssetMapLinks.ClearAndAddRange(allLinks.Values);
                Editor.Portfolios.ClearAndAddRange(allPortfolios.Values.OrderBy(a => a.Code));
                Editor.AssetMaps.ClearAndAddRange(allAssetMaps.Values.OrderBy(a => a.Name)
                    .Select(a => new AssetMapViewModel(a)));
                Editor.Indexes.ClearAndAddRange(allIndexes.Values.OrderBy(b => b.Code)
                    .ThenBy(b => b.Name));
            }
            Editor.InitializeMode(mode);
            Editor.IsReady = true;
        }

        private void ToggleShowCompositeBenchmarks()
        {
            if (Portfolios.IsNullOrEmpty())
                return;

            foreach (var ptf in Portfolios)
            {
                foreach (var cb in ptf.Children)
                {
                    if (cb.IsActive && !cb.IsExpired)
                        cb.IsVisible = true;
                    else if (!cb.IsActive && !cb.IsExpired)
                        cb.IsVisible = IsShowNonDefaultCompositeBenchmarks;
                    else if (cb.IsActive && cb.IsExpired)
                        cb.IsVisible = IsShowExpiredCompositeBenchmarks;
                    else if (!cb.IsActive && cb.IsExpired)
                        cb.IsVisible = IsShowNonDefaultCompositeBenchmarks && IsShowExpiredCompositeBenchmarks;
                }
            }

            if (SelectedPortfolioAssetMapLink != null)
            {
                if (!IsShowExpiredCompositeBenchmarks && SelectedPortfolioAssetMapLink.IsExpired)
                {
                    SelectedPortfolioAssetMapLink = null;
                }
            }
        }

        private void ToggleShowBenchmarkComponents()
        {
            var all = CompositeBenchmarkItems.Flatten(b => b.Children);
            foreach (var bcvm in all)
            {
                // always shows those Aggregated (parent) nodes and those nodes with benchmark.
                bcvm.IsVisible = IsShowAllBenchmarkComponents || bcvm.CompositeBenchmarkItem.IsReal || bcvm.IsAggregated;
            }
        }
    }
}