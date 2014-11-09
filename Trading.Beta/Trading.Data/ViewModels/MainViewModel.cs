using System.Diagnostics;
using MahApps.Metro.Controls;
using Ninject;
using Trading.Common;
using Trading.Common.Data;
using Trading.Common.SharedSettings;
using Trading.Common.Utils;
using Trading.Common.ViewModels;
using Trading.Data.Data;

namespace Trading.Data.ViewModels
{
    public partial class MainViewModel : MainViewModelBase<Access>, IMainViewModel
    {
        public IDatabaseViewModel DatabaseViewModel { get; set; }
        public IDownloadViewModel DownloadViewModel { get; set; }
        public IInteractiveBrokersViewModel IBViewModel { get; set; }

        public MainViewModel(
            IDatabaseViewModel databaseViewModel,
            IDownloadViewModel downloadViewModel,
            IInteractiveBrokersViewModel interactiveBrokersViewModel,
            IDataAccessFactory<Access> dataAccessFactory, ISettings settings)
            : base(dataAccessFactory, settings)
        {
            Constants.InitializeDirectories();

            DatabaseViewModel = databaseViewModel;
            DownloadViewModel = downloadViewModel;
            IBViewModel = interactiveBrokersViewModel;
        }

        protected override void OnLoaded(MetroWindow view)
        {
            ViewService.Window = view;

            DatabaseViewModel.ViewService = ViewService;
            DownloadViewModel.ViewService = ViewService;
            IBViewModel.ViewService = ViewService;

            DatabaseViewModel.DataAccessFactory = DataAccessFactory;

            DownloadViewModel.Initalize();
        }

        public override string ProgramName { get { return "Trading Data"; } }

        public void OpenPricesFolder()
        {
            Process.Start(Constants.PricesDirectory);
        }

        public void OpenSecuritiesFolder()
        {
            Process.Start(Constants.SecurityListDirectory);
        }

        public void OpenOtherDataFolder()
        {
            Process.Start(Constants.OtherDataDirectory);
        }

        public void OpenLogsFolder()
        {
            Process.Start(Constants.LogsDirectory);
        }
    }

    public interface IMainViewModel : IHasViewService
    {
    }
}