using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using Maintenance.Common;
using Maintenance.Common.Data;
using Maintenance.Common.SharedSettings;
using Maintenance.Common.Utils;
using Maintenance.Common.ViewModels;
using Portfolio = Maintenance.Portfolios.Entities.Portfolio;

namespace Maintenance.Portfolios.ViewModels
{
    public partial class MainViewModel : MainViewModelBase<PortfolioDataAccess, ImapSettings>,
        IMainViewModel, IHandle<ActivityMessage<IMainViewModel>>
    {
        public override sealed string ProgramName { get { return "Portfolio Maintenance"; } }

        public MainViewModel(IDataAccessFactory<PortfolioDataAccess> dataAccessFactory,
            IEditFlyoutViewModel editFlyout,
            IBenchmarkAssociationFlyoutViewModel benchmarkAssociationFlyout,
            IFilterFlyoutViewModel filterFlyout,
            IOptionsFlyoutViewModel optionsFlyout,
            ImapSettings settings)
            : base(dataAccessFactory, settings)
        {
            // init flyouts
            EditFlyout = editFlyout;
            BenchmarkAssociationFlyout = benchmarkAssociationFlyout;
            FilterFlyout = filterFlyout;
            OptionsFlyout = optionsFlyout;

            // init flyouts' da
            EditFlyout.DataAccessFactory = DataAccessFactory;
            BenchmarkAssociationFlyout.DataAccessFactory = DataAccessFactory;
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            EditFlyout.ViewService = ViewService;
            BenchmarkAssociationFlyout.ViewService = ViewService;
            Load();
        }

        public void About()
        {
            ViewService.ShowMessage("About " + ProgramName,
                "Version: " + Assembly.GetExecutingAssembly().GetName().Version
                + System.Environment.NewLine
                + "Database: " + Settings.GetOracleConnectionServiceName(environment));
        }

        public void HandleDoubleClick()
        {
            ToggleEditPortfolio();
        }

        public void HandleShortcutKeys(KeyEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                switch (e.Key)
                {
                    case Key.E: ToggleEditPortfolio(); break;
                    case Key.B: ToggleBenchmarkAssociation(); break;
                    case Key.F: ToggleFilter(); break;
                    case Key.R: Refresh(); break;
                    case Key.O: ToggleOptions(); break;
                }
            }

            if (e.Key == Key.Escape)
            {
                CloseAllFlyouts();
            }
        }

        public void CopySelected()
        {
            var value = GenerateClipboardContent(selectedItems);
            if (value != null)
                Clipboard.SetText(value);
        }

        public void CopyAll()
        {
            var value = GenerateClipboardContent(allPortfolios.Values);
            if (value != null)
                Clipboard.SetText(value);
        }

        public void ToggleEditPortfolio()
        {
            if (!CanToggleEditPortfolio)
                return;

            IsEditFlyoutOpened = !IsEditFlyoutOpened;

            if (IsEditFlyoutOpened)
            {
                if (!EditFlyout.IsReady)
                {
                    EditFlyout.SectorSchemes.ClearAndAddRange(allSectorSchemes);
                    EditFlyout.AssetClassFocuses.ClearAndAddRange(Settings.AssetClassFocuses);
                    EditFlyout.Locations.ClearAndAddRange(allCountries.Values.OrderBy(b => b.Code));
                }
                // copy is necessary, or else the AssetClassFocuses.Clear() (etc) will clear its existing value.
                EditFlyout.SetItem(SelectedPortfolio, SelectedPortfolioExtendedInfo == null ? null : SelectedPortfolioExtendedInfo.Copy());
                EditFlyout.IsReady = true;
            }
        }

        public void ToggleFilter()
        {
            if (!CanToggleFilter)
                return;

            IsFilterFlyoutOpened = !IsFilterFlyoutOpened;

            if (IsFilterFlyoutOpened)
            {
                if (!FilterFlyout.IsReady)
                {
                    FilterFlyout.ClearAllFields();
                }
                FilterFlyout.IsReady = true;
            }
        }

        public void ToggleBenchmarkAssociation()
        {
            if (!CanToggleBenchmarkAssociation)
                return;

            IsBenchmarkAssociationFlyoutOpened = !IsBenchmarkAssociationFlyoutOpened;

            if (IsBenchmarkAssociationFlyoutOpened)
            {
                if (!BenchmarkAssociationFlyout.IsReady)
                {
                    BenchmarkAssociationFlyout.Indexes.ClearAndAddRange(allIndexes.Values.OrderBy(b => b.Code));
                }
                BenchmarkAssociationFlyout.SetItem(SelectedPortfolio);
                BenchmarkAssociationFlyout.IsReady = true;
            }
        }

        public void ToggleOptions()
        {
            IsOptionsFlyoutOpened = !IsOptionsFlyoutOpened;

            OptionsFlyout.SelectedEnvironment = environment;
        }

        private void CloseAllFlyouts()
        {
            if (IsEditFlyoutOpened) ToggleEditPortfolio();
            if (IsBenchmarkAssociationFlyoutOpened) ToggleBenchmarkAssociation();
            if (IsFilterFlyoutOpened) ToggleFilter();
            if (IsOptionsFlyoutOpened) ToggleOptions();
        }

        public void SelectMultipleItems(IList items)
        {
            selectedItems = items;
            isSingleSelection = items != null && items.Count == 1;
            CheckEnabled();
        }

        private void CheckEnabled()
        {
            CanToggleBenchmarkAssociation = CanToggleEditPortfolio
                = SelectedPortfolio != null && isSingleSelection;
        }

        public void Handle(ActivityMessage<IMainViewModel> message)
        {
            switch (message.Type)
            {
                case ActivityType.Edit:
                    ToggleEditPortfolio();
                    Load();
                    break;
                case ActivityType.ChangePortfolioToBenchmark:
                    ToggleBenchmarkAssociation();
                    Load();
                    break;
                case ActivityType.Filter:
                    Filter(message.Item as FilterOptions);
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
                    if (message.Item == null || IsOptionsFlyoutOpened)
                    {
                        ToggleOptions();
                    }
                    break;
            }
        }

        public void Filter(FilterOptions options)
        {
            var items = new List<Portfolio>();
            items.AddRange(allPortfolios.Values);
            if (options != null && !options.IsReset)
            {
                foreach (var option in options)
                {
                    var value = option.Value;
                    switch (option.Key)
                    {
                        case "StartOfId":
                            if (!string.IsNullOrEmpty(value))
                                items.RemoveAll(i => !i.Id.ToString().StartsWithIgnoreCase(value));
                            break;
                        case "StartOfName":
                            if (!string.IsNullOrEmpty(value))
                                items.RemoveAll(i => !i.Name.StartsWithIgnoreCase(value));
                            break;
                        case "StartOfCode":
                            if (!string.IsNullOrEmpty(value))
                                items.RemoveAll(i => !i.Code.StartsWithIgnoreCase(value));
                            break;
                        case "StartOfPmName":
                            if (!string.IsNullOrEmpty(value))
                                items.RemoveAll(i => !i.PortfolioManagerName.StartsWithIgnoreCase(value));
                            break;
                        case "StartOfBmCode":
                            if (!string.IsNullOrEmpty(value))
                                items.RemoveAll(i => !i.IndexCode.StartsWithIgnoreCase(value));
                            break;
                        default:
                            throw new InvalidOperationException("Invalid filter option encountered: " + option.Key + ", " + option.Value);
                    }
                }
            }

            PopulatePortfolios(items);
        }

        private void PopulatePortfolios(IEnumerable<Portfolio> items)
        {
            Portfolios.ClearAndAddRange(items.OrderByDescending(p => p.IndexCode)
                .ThenBy(p => p.Code).ThenBy(p => p.Name));
        }

        private string GenerateClipboardContent(IEnumerable items)
        {
            if (items == null)
                return null;

            var builder = new StringBuilder()
                .AppendTab("Portfolio Id")
                .AppendTab("Portfolio Code")
                .AppendTab("Portfolio Name")
                .AppendTab("Portfolio Manager")
                .AppendTab("Benchmark Code")
                .AppendTab("Benchmark Type")
                .AppendTab("Benchmark Effective Date")
                .AppendLine("Benchmark Expiry Date");

            foreach (Portfolio item in items.OfType<Portfolio>().OrderBy(i => i.Id))
            {
                builder.AppendTab(item.Id)
                    .AppendTab(item.Code)
                    .AppendTab(item.Name)
                    .AppendTab(item.PortfolioManagerName)
                    .AppendTab(item.IndexCode)
                    .AppendTab(item.Benchmark == null ? string.Empty : item.Benchmark.Type)
                    .AppendTab(item.Benchmark == null
                        ? string.Empty
                        : item.Benchmark.EffectiveDate.IsoFormat())
                    .AppendLine(item.Benchmark == null
                        ? string.Empty
                        : item.Benchmark.ExpiryDate.IsoFormat());
            }

            return builder.ToString();
        }
    }
}