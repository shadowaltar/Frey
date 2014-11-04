using System.Diagnostics;
using Caliburn.Micro;
using System;
using System.IO;
using System.Linq;
using CsvHelper;
using Trading.Backtest.Data;
using Trading.Backtest.Reporting;
using Trading.Common;
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
            SelectedEndYear = 2004;
            if (!Directory.Exists(Constants.LogsDirectory))
            {
                Directory.CreateDirectory(Constants.LogsDirectory);
            }
        }

        public void Run()
        {
            core.SetDays(testStart, testEnd, endOfData);

            do
            {
                if (core.Positions.Count != 0)
                    core.ExitPositions();
                if (!core.EnterPositions())
                    break;
            } while (core.Next());
            core.Finish();
            core.CalculateStatistics();

            var resultPath = Path.Combine(Constants.LogsDirectory,
                "Trades[" + DateTime.Now.ToTimeDouble() + "].csv");
            using (var writer = new StreamWriter(resultPath))
            using (var csv = new CsvWriter(writer))
            {
                csv.WriteRecords(core.Trades.Select(t => new TradeReportEntry(t)));
            }

            Process.Start(resultPath);

            TestExcel();
        }

        public void TestExcel()
        {
            var reporter = new BacktestReport();
            reporter.Create(core);
        }
    }

    public interface IMainViewModel : IHasViewService, IHasDataAccessFactory<BacktestDataAccess>
    {
    }
}