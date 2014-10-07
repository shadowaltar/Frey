using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Maintenance.Common;
using Maintenance.Common.Data;
using Maintenance.Common.Entities;
using Maintenance.Common.Utils;
using Maintenance.Common.ViewModels;

namespace Maintenance.Benchmarks.ViewModels
{
    public partial class MainViewModel : ViewModelBase, IMainViewModel, IHandle<ActivityMessage<IMainViewModel>>
    {
        public static readonly string ProgramName = "Benchmark Maintenance";

        public MainViewModel(IEventAggregator eventAggregator,
            IViewService viewService, IDataAccessFactory dataAccessFactory,
            IAddFlyoutViewModel addFlyout, IEditFlyoutViewModel editFlyout, IFilterFlyoutViewModel filterFlyout,
            IOptionsFlyoutViewModel optionsFlyout, IBenchmarkDependencyReportViewModel dependencyReport)
        {
            ViewService = viewService;

            EventAggregator = eventAggregator;
            EventAggregator.Subscribe(this);

            AddFlyout = addFlyout;
            EditFlyout = editFlyout;
            FilterFlyout = filterFlyout;
            OptionsFlyout = optionsFlyout;
            this.dependencyReport = dependencyReport;

            DataAccessFactory = dataAccessFactory;
            DataAccessFactory.Environments.AddRange(Settings.Environments);
            DataAccessFactory.Environment = environment;

            AddFlyout.DataAccessFactory = DataAccessFactory;
            EditFlyout.DataAccessFactory = DataAccessFactory;

            DisplayName = string.Format("{0} ({1})", ProgramName, environment);
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            ViewService.Window = (MetroWindow)view;
            ViewService.ToFront();

            AddFlyout.ViewService = ViewService;
            EditFlyout.ViewService = ViewService;

            Load();
        }

        public void SelectMultipleItems(IList items)
        {
            selectedItems = items;
            isSingleSelection = items != null && items.Count == 1;
            CheckEnabled();
        }

        private void CheckEnabled()
        {
            CanToggleEdit = CanDelete
                = SelectedBenchmark != null && isSingleSelection;
            EditFlyout.IsReady = false;
        }

        private void RegisterMissingIndex(long indexId)
        {
            allIndexes[indexId] = new Index
            {
                Id = indexId,
                Code = indexId.ToString(),
                Name = indexId.ToString()
            };
        }

        public void ToggleAdd()
        {
            IsAddFlyoutOpen = !IsAddFlyoutOpen;
            if (IsAddFlyoutOpen && !AddFlyout.IsReady)
            {
                AddFlyout.ClearAll();
                AddFlyout.Types.ClearAndAddRange(availableBenchmarkTypes);
                AddFlyout.Indexs.ClearAndAddRange(allIndexes.Values.OrderBy(i => i.Code));
                AddFlyout.ExistingItems = allBenchmarks.Values;
                AddFlyout.CanSave = false;
                AddFlyout.IsReady = true;
            }
        }

        public void ToggleEdit()
        {
            IsEditFlyoutOpen = !IsEditFlyoutOpen;
            if (IsEditFlyoutOpen && !EditFlyout.IsReady)
            {
                EditFlyout.Types.Clear();
                EditFlyout.Types.ClearAndAddRange(availableBenchmarkTypes);
                EditFlyout.Indexs.ClearAndAddRange(allIndexes.Values.OrderBy(i => i.Code));
                EditFlyout.ExistingItems = allBenchmarks.Values;
                EditFlyout.SetItem(SelectedBenchmark);
                EditFlyout.CanSave = false;
                EditFlyout.IsReady = true;
            }
        }

        public void ToggleFilter()
        {
            IsFilterFlyoutOpen = !IsFilterFlyoutOpen;
            if (IsFilterFlyoutOpen && !FilterFlyout.IsReady)
            {
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

            // ask confirmation
            var decision = await ViewService.AskDelete();
            if (decision != MessageDialogResult.Affirmative)
                return;

            // scan dependency; if cannot get, show warning and ask if wanna abort
            bool stopByDependency = false;
            try
            {
                var dependencies = await LoadBenchmarkDependencies();
                if (!dependencies.IsNullOrEmpty())
                {
                    stopByDependency = true;

                    dependencyReport.Dependencies.ClearAndAddRange(dependencies);
                    await ViewService.ShowDialog(dependencyReport as ViewModelBase);
                }
            }
            catch (Exception e)
            {
                Log.Error("Cannot get the benchmark dependencies (portfolios and/or composite benchmarks).", e);
            }
            if (stopByDependency)
                return;

            // do delete
            var result = await TaskEx.Run(() =>
            {
                try
                {
                    Log.InfoFormat("Delete item in database: {0}", SelectedBenchmark);

                    using (var da = DataAccessFactory.New())
                        return da.DeleteBenchmark(SelectedBenchmark.Code);
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
                    "The benchmark \"" + SelectedBenchmark.Code + "\" is deleted.");
                Refresh();
            }
            else
            {
                await ViewService.ShowError("Failed to delete the entry from database; " +
                                            "try refreshing or consult the development team for diagnosis.");
            }
        }

        public new void Refresh()
        {
            Load();
        }

        public void About()
        {
            ViewService.ShowMessage("About " + ProgramName,
                "Version: " + Assembly.GetExecutingAssembly().GetName().Version + Environment.NewLine + "By Mars");
        }

        public void HandleShortcutKeys(KeyEventArgs e)
        {
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                switch (e.Key)
                {
                    case Key.N: ToggleAdd(); break;
                    case Key.F: ToggleFilter(); break;
                    case Key.D: Delete(); break;
                    case Key.E: ToggleEdit(); break;
                    case Key.R: Refresh(); break;
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

        public void CopySelected()
        {
            var value = GenerateClipboardContent(selectedItems);
            if (value != null)
                Clipboard.SetText(value);
        }

        public void CopyAll()
        {
            var value = GenerateClipboardContent(benchmarks);
            if (value != null)
                Clipboard.SetText(value);
        }

        public void Handle(ActivityMessage<IMainViewModel> message)
        {
            if (message == null)
                return;
            switch (message.Type)
            {
                case ActivityType.Add:
                case ActivityType.Edit:
                    CloseAllFlyouts();
                    Refresh();
                    break;
                case ActivityType.Filter:
                    Filter(message.Item as FilterOptions);
                    break;
                case ActivityType.ChangeEnvironment:
                    if (message.Item != null)
                    {
                        environment = (string)message.Item;
                        if (DataAccessFactory.Environment != environment)
                        {
                            DataAccessFactory.Environment = environment;
                            DisplayName = string.Format("{0} ({1})", ProgramName, environment);
                            Refresh();
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
            var items = new List<Benchmark>();
            items.AddRange(allBenchmarks.Values);
            if (!options.IsReset)
            {
                foreach (var option in options)
                {
                    var value = option.Value;
                    switch (option.Key)
                    {
                        case "StartOfName":
                            if (!string.IsNullOrEmpty(value))
                                items.RemoveAll(i => !i.Name.StartsWithIgnoreCase(value));
                            break;
                        case "StartOfCode":
                            if (!string.IsNullOrEmpty(value))
                                items.RemoveAll(i => !i.Code.StartsWithIgnoreCase(value));
                            break;
                        default:
                            throw new InvalidOperationException("Invalid filter option encountered: "
                                + option.Key + ", " + option.Value);
                    }
                }
            }
            CheckEnabled();
            Benchmarks.ClearAndAddRange(items);
        }

        private string GenerateClipboardContent(IList items)
        {
            if (items == null)
                return null;

            var builder = new StringBuilder()
                .AppendTab("Bm Id")
                .AppendTab("Bm Code")
                .AppendTab("Bm Name")
                .AppendTab("Type")
                .AppendTab("Index Id")
                .AppendTab("Index Code")
                .AppendTab("Index Name")
                .AppendTab("Portfolio Id")
                .AppendTab("Portfolio Code")
                .AppendLine("Portfolio Name");

            foreach (Benchmark item in items)
            {
                builder.AppendTab(item.Id)
                    .AppendTab(item.Code)
                    .AppendTab(item.Name)
                    .AppendTab(item.Type)
                    .AppendTab(item.Type == "INDEX" ? item.BasedOn.Id.ToString() : string.Empty)
                    .AppendTab(item.Type == "INDEX" ? item.BasedOn.Code : string.Empty)
                    .AppendTab(item.Type == "INDEX" ? item.BasedOn.Name : string.Empty)
                    .AppendTab(item.Type == "PORTFOLIO" ? item.BasedOn.Id.ToString() : string.Empty)
                    .AppendTab(item.Type == "PORTFOLIO" ? item.BasedOn.Code : string.Empty)
                    .AppendLine(item.Type == "PORTFOLIO" ? item.BasedOn.Name : string.Empty);
            }

            return builder.ToString();
        }
    }
}