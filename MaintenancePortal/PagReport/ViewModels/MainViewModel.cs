using System.Reflection;
using Caliburn.Micro;
using Maintenance.Common;
using Maintenance.Common.Data;
using Maintenance.Common.SharedSettings;
using Maintenance.Common.Utils;
using Maintenance.Common.ViewModels;

namespace Maintenance.PagReport.ViewModels
{
    public partial class MainViewModel : MainViewModelBase<PagReportDataAccess, ImapSettings>,
        IMainViewModel, IHandle<ActivityMessage<IMainViewModel>>
    {
        public override sealed string ProgramName { get { return "PAG Report Maintenance"; } }

        public MainViewModel(IDataAccessFactory<PagReportDataAccess> dataAccessFactory,
            ImapSettings settings)
            : base(dataAccessFactory, settings)
        {
            IsTabsEnabled = true;
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            Load();
        }

        public void ToggleEdit()
        {
            if (!CanToggleEdit)
                return;

            //if (IsInIndexView && SelectedIndex != null)
            //    IsEditIndexFlyoutOpened =true;
            //else if (SelectedFund != null)
            //    IsEditFundFlyoutOpen = !IsEditFundFlyoutOpen;

            //if (IsEditFundFlyoutOpen)
            //{
            //    EditFundFlyout.SetItem(SelectedFund);
            //    EditFundFlyout.IsReady = true;
            //}
            //else if (IsEditBenchmarkFlyoutOpen)
            //{
            //    EditBenchmarkFlyout.SetItem(SelectedBenchmark);
            //    EditBenchmarkFlyout.IsReady = true;
            //}
        }

        public void About()
        {
            ViewService.ShowMessage("About " + ProgramName,
                "Version: " + Assembly.GetExecutingAssembly().GetName().Version
                + System.Environment.NewLine
                + "Database: " + Settings.GetOracleConnectionServiceName(environment));
        }

        public void Handle(ActivityMessage<IMainViewModel> message)
        {
        }
    }
}