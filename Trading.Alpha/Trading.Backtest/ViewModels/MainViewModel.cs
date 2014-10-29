using System;
using Caliburn.Micro;
using MahApps.Metro.Controls.Dialogs;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Trading.Backtest.Data;
using Trading.Common.Data;
using Trading.Common.SharedSettings;
using Trading.Common.Utils;
using Trading.Common.ViewModels;

namespace Trading.Backtest.ViewModels
{
    public partial class MainViewModel : MainViewModelBase<BacktestDataAccess>, IMainViewModel
    {
        public MainViewModel(IDataAccessFactory<BacktestDataAccess> dataAccessFactory, ISettings settings)
            : base(dataAccessFactory, settings)
        {
            DataAccessFactory = dataAccessFactory;
            StartYears = new BindableCollection<int>(Enumerable.Range(2000, 15));
            EndYears = new BindableCollection<int>(Enumerable.Range(2000, 15));
            SelectedStartYear = 2004;
            SelectedEndYear = 2014;
        }

        public override string ProgramName { get { return "Backtest"; } }

        private ProgressDialogController progressIndicator;

        public BindableCollection<int> StartYears { get; private set; }
        public BindableCollection<int> EndYears { get; private set; }

        private int selectedStartYear;
        public int SelectedStartYear { get { return selectedStartYear; } set { SetNotify(ref selectedStartYear, value); } }
        private int selectedEndYear;
        public int SelectedEndYear { get { return selectedEndYear; } set { SetNotify(ref selectedEndYear, value); } }

        public async void Initialize()
        {
            // get prices 
            progressIndicator = await ViewService.ShowProgress("Loading data", "", true);
            await Task.WhenAll(GetSecurityVolumeInfo(), GetAllSecurities());
            await progressIndicator.Stop();
        }

        private BacktestDataAccess commonAccess;
        private MySqlCommand commonCommand;
        public async void Run()
        {
            if (commonAccess == null)
                commonAccess = DataAccessFactory.New();
            if (commonCommand == null)
                commonCommand = commonAccess.GetCommonCommand();

            var startDate = GetStartDateWithData();

            var results = commonAccess.GetTwoDaysTopVolumes(startDate);
        }
    }

    public interface IMainViewModel : IHasViewService, IHasDataAccessFactory<BacktestDataAccess>
    {
    }
}