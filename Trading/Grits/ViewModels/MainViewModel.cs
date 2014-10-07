using Caliburn.Micro;
using MahApps.Metro.Controls;
using Maintenance.Common;
using Maintenance.Common.Data;
using Maintenance.Common.SharedSettings;
using Maintenance.Common.Utils;
using Maintenance.Common.ViewModels;
using System.Collections;
using System.Reflection;
using System.Windows.Input;

namespace Maintenance.Grits.ViewModels
{
    public partial class MainViewModel : MainViewModelBase<GritsDataAccess, FmsSettings>,
        IMainViewModel, IHandle<ActivityMessage<IMainViewModel>>
    {
        public override sealed string ProgramName { get { return "Grits Maintenance"; } }

        public MainViewModel(IAddFlyoutViewModel addFlyout,
            IEditFundFlyoutViewModel editFundFlyout,
            IEditBenchmarkFlyoutViewModel editBenchmarkFlyout,
            IOptionsFlyoutViewModel optionsFlyout,
            IDataAccessFactory<GritsDataAccess> dataAccessFactory,
            FmsSettings settings)
            : base(dataAccessFactory, settings)
        {
            AddFlyout = addFlyout;
            EditFundFlyout = editFundFlyout;
            EditBenchmarkFlyout = editBenchmarkFlyout;
            OptionsFlyout = optionsFlyout;

            EditBenchmarkFlyout.DataAccessFactory = dataAccessFactory;
            EditFundFlyout.DataAccessFactory = dataAccessFactory;
            AddFlyout.DataAccessFactory = dataAccessFactory;
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            EditBenchmarkFlyout.ViewService = ViewService;
            EditFundFlyout.ViewService = ViewService;
            AddFlyout.ViewService = ViewService;

            Load();
        }

        public void About()
        {
            ViewService.ShowMessage("About " + ProgramName,
                "Version: " + Assembly.GetExecutingAssembly().GetName().Version
                + System.Environment.NewLine + "Database: " + environment);
        }

        public void ToggleAdd()
        {
            if (!CanToggleAdd)
                return;

            IsAddFlyoutOpen = !IsAddFlyoutOpen;

            if (IsAddFlyoutOpen && !AddFlyout.IsReady)
            {
                AddFlyout.ExistingGritsFunds = allGritsFunds;
                AddFlyout.ExistingGritsBenchmarks = allGritsBenchmarks;

                AddFlyout.Funds.ClearAndAddRange(allFunds);
                AddFlyout.Benchmarks.ClearAndAddRange(allBenchmarks);

                AddFlyout.IsReady = true;
            }
        }

        public void ToggleEdit()
        {
            if (!CanToggleEdit)
                return;

            if (SelectedBenchmark != null)
                IsEditBenchmarkFlyoutOpen = !IsEditBenchmarkFlyoutOpen;
            else if (SelectedFund != null)
                IsEditFundFlyoutOpen = !IsEditFundFlyoutOpen;

            if (IsEditFundFlyoutOpen)
            {
                EditFundFlyout.SetItem(SelectedFund);
                EditFundFlyout.IsReady = true;
            }
            else if (IsEditBenchmarkFlyoutOpen)
            {
                EditBenchmarkFlyout.SetItem(SelectedBenchmark);
                EditBenchmarkFlyout.IsReady = true;
            }
        }

        public void ToggleOptions()
        {
            IsOptionsFlyoutOpen = !IsOptionsFlyoutOpen;

            OptionsFlyout.SelectedEnvironment = environment;
        }

        public void HandleDoubleClick()
        {
            ToggleEdit();
        }

        public void HandleShortcutKeys(KeyEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                switch (e.Key)
                {
                    case Key.N: ToggleAdd(); break;
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

        public void SelectMultipleItems(IList items)
        {
        }

        /// <summary>
        /// Handle the post-operation tasks here, like refreshing the grid.
        /// </summary>
        /// <param name="message"></param>
        public void Handle(ActivityMessage<IMainViewModel> message)
        {
            switch (message.Type)
            {
                case ActivityType.Add:
                    ToggleAdd();
                    Load();
                    break;
                case ActivityType.Edit:
                    ToggleEdit();
                    Load();
                    break;
                case ActivityType.ChangeEnvironment:
                    if (message.Item != null)
                    {
                        environment = (string)message.Item;
                        if (DataAccessFactory.Environment != environment)
                        {
                            DataAccessFactory.Environment = environment;
                            DisplayName = string.Format("{0} ({1})", ProgramName, environment);
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

        private void CloseAllFlyouts()
        {
            if (IsAddFlyoutOpen) ToggleAdd();
            if (IsEditBenchmarkFlyoutOpen) ToggleEdit();
            if (IsEditFundFlyoutOpen) ToggleEdit();
            if (IsOptionsFlyoutOpen) ToggleOptions();
        }
    }
}