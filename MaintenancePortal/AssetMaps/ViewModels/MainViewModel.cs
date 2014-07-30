using Caliburn.Micro;
using MahApps.Metro.Controls;
using Maintenance.AssetMaps.Entities;
using Maintenance.Common;
using Maintenance.Common.Data;
using Maintenance.Common.Entities;
using Maintenance.Common.Entities.Decorators;
using Maintenance.Common.SharedSettings;
using Maintenance.Common.Utils;
using Maintenance.Common.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Input;

namespace Maintenance.AssetMaps.ViewModels
{
    public partial class MainViewModel : MainViewModelBase<AssetMapDataAccess, ImapSettings>,
        IMainViewModel, IHandle<ActivityMessage<IMainViewModel>>
    {
        public override sealed string ProgramName { get { return "Asset Map Maintenance"; } }

        public MainViewModel(IOptionsFlyoutViewModel optionsFlyout,
            IDataAccessFactory<AssetMapDataAccess> dataAccessFactory,
            IEditorViewModel editor,
            ImapSettings settings)
            : base(dataAccessFactory, settings)
        {
            Editor = editor;
            Editor.DataAccessFactory = DataAccessFactory;

            OptionsFlyout = optionsFlyout;

            IsPortfolioListVisible = true;
            IsAssetMapListVisible = false;
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            Load();
        }

        public void About()
        {
            ViewService.ShowMessage("About " + ProgramName,
                "Version: " + Assembly.GetExecutingAssembly().GetName().Version
                + System.Environment.NewLine
                + "Database: " + Settings.GetOracleConnectionServiceName(environment));
        }

        public void Add()
        {
            Editor.Initialize(EditorViewModel.EditorMode.Create);

            Editor.RelatedPortfolios = null;
            Editor.AllAssetMaps = allAssetMaps.Values;
            Editor.AllAssetMapComponents = allComponents.Values;
            Editor.CheckNonDeletableAsmComponent(nonDeletableAsmComponentIds);

            ViewService.ShowWindow(Editor as ViewModelBase);
        }

        public void Edit()
        {
            Editor.Initialize(EditorViewModel.EditorMode.Edit, SelectedAssetMap);

            Editor.RelatedPortfolios = FindPortfoliosByAssetMap(SelectedAssetMap);
            Editor.AllAssetMaps = allAssetMaps.Values;
            Editor.AllAssetMapComponents = allComponents.Values;
            Editor.CheckNonDeletableAsmComponent(nonDeletableAsmComponentIds);

            ViewService.ShowWindow(Editor as ViewModelBase);
        }

        public void ToggleOptions()
        {
            IsOptionsFlyoutOpen = !IsOptionsFlyoutOpen;

            OptionsFlyout.SelectedEnvironment = environment;
        }

        public void HandleDoubleClick(object focusedItem)
        {
            var pvm = focusedItem as PortfolioViewModel;
            if (pvm != null)
            {
                Edit();
                return;
            }

            var asm = focusedItem as AssetMap;
            if (asm != null)
            {
                Edit();
            }
        }

        public void HandleShortcutKeys(KeyEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                switch (e.Key)
                {
                    case Key.C:
                        Add();
                        break;
                    case Key.E:
                        if (SelectedAssetMap != null)
                            Edit();
                        break;
                    case Key.R:
                        Refresh();
                        break;
                    case Key.O:
                        ToggleOptions();
                        break;
                }
            }
        }

        public void SelectComponent(AssetMapComponentViewModel selected)
        {
            SelectedComponent = selected;
            ComponentProperties.Clear();
            if (SelectedComponent != null)
            {
                ComponentProperties.AddRange(SelectedComponent.Component.Properties
                    .Select(p => new ComponentPropertyViewModel(p)).OrderBy(p => p.Key));
            }
        }

        public void SelectPortfolio(object selected)
        {
            if (selected == null)
            {
                CanEdit = false;
                return;
            }

            if (selected is PortfolioManagerViewModel)
            {
                SelectedPortfolioManager = ((PortfolioManagerViewModel)selected).PortfolioManager;
                CanEdit = false;
            }
            else if (selected is PortfolioViewModel)
            {
                CanEdit = true;
                Portfolio portfolio = ((PortfolioViewModel)selected).Portfolio;
                if (SelectedPortfolio != portfolio)
                {
                    SelectedPortfolio = ((PortfolioViewModel)selected).Portfolio;
                    SelectedPortfolioManager = SelectedPortfolio.PortfolioManager;

                    SelectedAssetMap = SelectedPortfolio.AssetMap;

                    InitializeAssetMapStructure();
                }
            }
        }

        public void ExpandAllPortfolios()
        {
            foreach (var pmvm in PortfolioManagers)
            {
                pmvm.IsExpanded = true;
            }
        }

        public void CollapseAllPortfolios()
        {
            foreach (var pmvm in PortfolioManagers)
            {
                pmvm.IsExpanded = false;
            }
        }

        public void ExpandAllNodes()
        {
            ToggleAllNodesExpanded(AssetMapComponents, true);
        }

        public void CollapseAllNodes()
        {
            ToggleAllNodesExpanded(AssetMapComponents, false);
        }

        public void Handle(ActivityMessage<IMainViewModel> message)
        {
            switch (message.Type)
            {
                case ActivityType.Edit:
                    Load();
                    break;
                case ActivityType.ChangeEnvironment:
                    if (message.Item != null)
                    {
                        if (DataAccessFactory.Environment != (string)message.Item)
                        {
                            Environment = (string)message.Item;
                            Load();
                        }
                    }
                    if (message.Item == null || IsOptionsFlyoutOpen)
                    {
                        ToggleOptions();
                    }
                    break;
            }
        }

        private void InitializeAssetMapStructure()
        {
            AssetMapComponents.Clear();
            if (SelectedAssetMap != null)
            {
                AssetMapComponents.Add(new AssetMapComponentViewModel(SelectedAssetMap.RootComponent));
                ExpandSecondLevelNodes(AssetMapComponents);
            }
        }

        private List<MapsPortfolio> FindPortfoliosByAssetMap(AssetMap assetMap)
        {
            if (assetMap == null)
                return null;
            return allPortfolios.Values.Where(p => p.AssetMap != null && p.AssetMap.Id == assetMap.Id).ToList();
        }

        private void ExpandSecondLevelNodes<T>(IEnumerable<IHasChildNodes<T>> items)
        {
            foreach (var item in items)
            {
                foreach (ITreeNode child in item.Children.OfType<ITreeNode>())
                {
                    child.IsExpanded = true;
                }
            }
        }

        private void ToggleAllNodesExpanded(IEnumerable<AssetMapComponentViewModel> nodes, bool isExpanded)
        {
            foreach (var acvm in nodes.Flatten(vm => vm.Children))
            {
                acvm.IsExpanded = isExpanded;
            }
        }
    }
}