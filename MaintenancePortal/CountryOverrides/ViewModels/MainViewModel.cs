using Caliburn.Micro;
using MahApps.Metro.Controls.Dialogs;
using Maintenance.Common.Data;
using Maintenance.Common.Entities;
using Maintenance.Common.SharedSettings;
using Maintenance.Common.Utils;
using Maintenance.Common.ViewModels;
using Maintenance.CountryOverrides.Entities;
using Maintenance.CountryOverrides.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Maintenance.CountryOverrides.ViewModels
{
    public partial class MainViewModel : MainViewModelBase<CountryOverrideDataAccess, ImapSettings>,
        IMainViewModel, IHandle<ActivityMessage<IMainViewModel>>
    {
        public override sealed string ProgramName { get { return "Country Override Maintenance"; } }

        public MainViewModel(IAddFlyoutViewModel addFlyout,
            IEditFlyoutViewModel editFlyout,
            ICustomCountryEditorViewModel customCountryEditor,
            IFilterFlyoutViewModel filterFlyout,
            IOptionsFlyoutViewModel optionsFlyout,
            IDataAccessFactory<CountryOverrideDataAccess> dataAccessFactory,
            ImapSettings settings)
            : base(dataAccessFactory, settings)
        {
            AddFlyout = addFlyout;
            EditFlyout = editFlyout;
            CustomCountryEditor = customCountryEditor;
            FilterFlyout = filterFlyout;
            OptionsFlyout = optionsFlyout;

            AddFlyout.DataAccessFactory = DataAccessFactory;
            EditFlyout.DataAccessFactory = DataAccessFactory;
            CustomCountryEditor.DataAccessFactory = DataAccessFactory;
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            AddFlyout.ViewService = ViewService;
            EditFlyout.ViewService = ViewService;
            Load();
        }

        public void About()
        {
            ViewService.ShowMessage("About " + ProgramName,
                "Version: " + Assembly.GetExecutingAssembly().GetName().Version
                + System.Environment.NewLine
                + "Database: " + Settings.GetOracleConnectionServiceName(environment));
        }

        public void ToggleAdd()
        {
            if (!CanToggleAdd)
                return;

            IsAddFlyoutOpen = !IsAddFlyoutOpen;

            if (IsAddFlyoutOpen && !AddFlyout.IsReady)
            {
                AddFlyout.ExistingItems = allItems;

                AddFlyout.Countries.ClearAndAddRange(orderedCountries);
                AddFlyout.Portfolios.ClearAndAddRange(orderedPortfolios);
                AddFlyout.PortfolioManagers.ClearAndAddRange(orderedPortfolioManagers);

                AddFlyout.IsReady = true;
            }
        }

        public void ToggleEdit()
        {
            if (!CanToggleEdit)
                return;

            IsEditFlyoutOpen = !IsEditFlyoutOpen;

            if (IsEditFlyoutOpen && !EditFlyout.IsReady)
            {
                EditFlyout.Countries.ClearAndAddRange(orderedCountries);
                EditFlyout.Portfolios.ClearAndAddRange(orderedPortfolios);
                EditFlyout.PortfolioManagers.ClearAndAddRange(orderedPortfolioManagers);
                EditFlyout.SetItem(SelectedOverrideItem);

                EditFlyout.IsReady = true;
            }
        }

        public void ToggleFilter()
        {
            IsFilterFlyoutOpen = !IsFilterFlyoutOpen;

            if (IsFilterFlyoutOpen && !FilterFlyout.IsReady)
            {
                FilterFlyout.OriginalCountries.ClearAndAddRange(orderedCountries);
                FilterFlyout.OriginalCountries.Insert(0, Dummies.NewAnyCountry);
                FilterFlyout.Portfolios.ClearAndAddRange(orderedPortfolios);
                FilterFlyout.Portfolios.Insert(0, Portfolio.Any);
                FilterFlyout.PortfolioManagers.ClearAndAddRange(orderedPortfolioManagers);
                FilterFlyout.PortfolioManagers.Insert(0, PortfolioManager.Any);

                FilterFlyout.ClearAll();
                FilterFlyout.IsReady = true;
            }
        }

        public void ToggleOptions()
        {
            IsOptionsFlyoutOpen = !IsOptionsFlyoutOpen;

            OptionsFlyout.SelectedEnvironment = environment;
        }

        public async void Delete()
        {
            if (!CanDelete)
                return;

            var decision = await ViewService.ShowMessage("Confirm to delete?",
                    "Remember you can't undo, and applications would immediately make use of your modification.",
                    false, true, "Yes", "No");

            if (decision != MessageDialogResult.Affirmative)
                return;

            var result = await TaskEx.Run(() =>
            {
                try
                {
                    Log.InfoFormat("Delete item in database: {0}", SelectedOverrideItem);

                    using (var da = DataAccessFactory.New())
                    {
                        return da.Delete(SelectedOverrideItem);
                    }
                }
                catch (Exception e)
                {
                    Log.Error("Error occurs when deleting data from database.", e);
                    return false;
                }
            });

            if (result)
            {
                await ViewService.ShowMessage("Entry deleted",
                    "The override entry for \"" + SelectedOverrideItem.Name + "\" is deleted.");
                Load();
            }
            else
            {
                await ViewService.ShowError("Failed to delete the entry from database; " +
                                            "try refreshing or consult the development team for diagnosis.");
            }
        }

        public void OpenCustomCountryEditor()
        {
            ViewService.ShowIndependentWindow((ViewModelBase)CustomCountryEditor);
        }

        public void HandleShortcutKeys(KeyEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                switch (e.Key)
                {
                    case Key.N: ToggleAdd(); break;
                    case Key.F: ToggleFilter(); break;
                    case Key.D: Delete(); break;
                    case Key.E: ToggleEdit(); break;
                    case Key.R: Load(); break;
                    case Key.O: ToggleOptions(); break;
                }
            }

            if (e.Key == Key.Escape)
            {
                CloseAllFlyouts();
            }
        }

        private void CloseAllFlyouts()
        {
            if (IsAddFlyoutOpen) ToggleAdd();
            if (IsEditFlyoutOpen) ToggleEdit();
            if (IsFilterFlyoutOpen) ToggleFilter();
            if (IsOptionsFlyoutOpen) ToggleOptions();
        }

        public void SelectMultipleItems(IList items)
        {
            selectedItems = items;
            isOnlyOneSelected = items != null && items.Count == 1;

            CheckEnabledAndVisibility();
        }

        public void CopySelected()
        {
            var value = GenerateClipboardContent(selectedItems);
            if (value != null)
                Clipboard.SetText(value);
        }

        public void CopyAll()
        {
            var value = GenerateClipboardContent(allItems);
            if (value != null)
                Clipboard.SetText(value);
        }

        public void HandleDoubleClick()
        {
            ToggleEdit();
        }

        /// <summary>
        /// Handle the post-operation tasks here, like refreshing the grid.
        /// </summary>
        /// <param name="message"></param>
        public void Handle(ActivityMessage<IMainViewModel> message)
        {
            if (message == null)
                return;
            switch (message.Type)
            {
                case ActivityType.Add:
                case ActivityType.Edit:
                    CloseAllFlyouts();
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
                    if (message.Item == null || IsOptionsFlyoutOpen)
                    {
                        ToggleOptions();
                    }
                    break;
                // no OperationType.Delete; because main vm itself does the deletion.
            }
        }

        private void Filter(FilterOptions options)
        {
            var items = new List<OverrideItem>();
            items.AddRange(allItems);
            if (!options.IsReset)
            {
                foreach (var option in options)
                {
                    var value = option.Value;
                    switch (option.Key)
                    {
                        case "None":
                            items.AddRange(allItems);
                            break;
                        case "StartOfId":
                            if (!string.IsNullOrEmpty(value))
                                items.RemoveAll(i => !i.Id.ToString().StartsWith(value, true, CultureInfo.InvariantCulture));
                            break;
                        case "StartOfName":
                            if (!string.IsNullOrEmpty(value))
                                items.RemoveAll(i => !i.Name.StartsWith(value, true, CultureInfo.InvariantCulture));
                            break;
                        case "OverrideType":
                            if (!string.IsNullOrEmpty(value) && value != OverrideType.Any.ToString())
                                items.RemoveAll(i => i.Type.ToString() != value);
                            break;
                        case "OriginalCountry":
                            if (!string.IsNullOrEmpty(value))
                                items.RemoveAll(i => i.OldCountryCode != value);
                            break;
                        case "NewCountry":
                            if (!string.IsNullOrEmpty(value))
                                items.RemoveAll(i => i.NewCountryCode != value);
                            break;
                        case "StartOfCusip":
                            if (!string.IsNullOrEmpty(value))
                                items.RemoveAll(i => !i.Cusip.StartsWith(value, true, CultureInfo.InvariantCulture));
                            break;
                        case "StartOfSedol":
                            if (!string.IsNullOrEmpty(value))
                                items.RemoveAll(i => !i.Sedol.StartsWith(value, true, CultureInfo.InvariantCulture));
                            break;
                        case "PortfolioManagerId":
                            if (!string.IsNullOrEmpty(value) && value != "-1")
                                items.RemoveAll(i => i.PortfolioManagerId != Convert.ToInt64(value));
                            break;
                        case "PortfolioId":
                            if (!string.IsNullOrEmpty(value) && value != "-1")
                                items.RemoveAll(i => i.PortfolioId != Convert.ToInt64(value));
                            break;
                        default:
                            throw new InvalidOperationException("Invalid filter option encountered: " + option.Key + ", " + option.Value);
                    }
                }
            }
            CheckEnabledAndVisibility();
            OverrideItems.ClearAndAddRange(items);
        }

        private void CheckEnabledAndVisibility()
        {
            if (SelectedOverrideItem == null)
            {
                CanToggleEdit = isOnlyOneSelected;
                CanDelete = isOnlyOneSelected;
                IsBelongsToVisible = false;
                EditFlyout.IsReady = false;
                return;
            }

            var type = SelectedOverrideItem.Type;
            switch (type)
            {
                case OverrideType.FILC:
                    CanToggleEdit = false;
                    CanDelete = false;
                    IsBelongsToVisible = false;
                    break;
                case OverrideType.ALL:
                    CanToggleEdit = isOnlyOneSelected;
                    CanDelete = isOnlyOneSelected;
                    IsBelongsToVisible = false;
                    EditFlyout.IsReady = false;
                    break;
                case OverrideType.PM:
                case OverrideType.PORTFOLIO:
                    CanToggleEdit = isOnlyOneSelected;
                    CanDelete = isOnlyOneSelected;
                    IsBelongsToVisible = true;
                    EditFlyout.IsReady = false;
                    break;
                case OverrideType.Any:
                    throw new InvalidOverrideTypeException(type);
            }
        }

        private string GenerateClipboardContent(IList items)
        {
            if (items == null)
                return null;

            var builder = new StringBuilder()
                .AppendTab("Tradable Entity Id")
                .AppendTab("Security Name")
                .AppendTab("Original Country")
                .AppendTab("New Country")
                .AppendTab("Type")
                .AppendTab("PortfolioManager")
                .AppendLine("Portfolio");

            foreach (OverrideItem item in items)
            {
                builder.AppendTab(item.Id)
                    .AppendTab(item.Name)
                    .AppendTab(item.OldCountryName)
                    .AppendTab(item.NewCountryName)
                    .AppendTab(item.Type)
                    .AppendTab(item.PortfolioManagerName)
                    .AppendLine(item.PortfolioName);
            }

            return builder.ToString();
        }
    }
}