using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Trading.Backtest.Data;
using Trading.Common.Data;
using Trading.Common.Entities;
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
        }

        public override string ProgramName
        {
            get { return "Backtest"; }
        }

        public async void Test()
        {
            var startTime = new DateTime(2004, 1, 1);
            var endTime = new DateTime(2014, 1, 1);
            var currentTime = startTime;
            var periodEndDate = startTime.AddYears(1);

            var results = new Dictionary<DateTime, Dictionary<int, double>>();
            // day looping
            await Task.Run(() =>
            {
                using (var access = DataAccessFactory.New())
                using (var cmd = new MySqlCommand())
                {
                    while (currentTime < endTime)
                    {
                        using (ReportTime.Start("1Y time: {0}"))
                        {
                            var volumes = PrepareSecurities(cmd, access, currentTime, periodEndDate);
                            foreach (var p in volumes)
                            {
                                results[p.Key] = p.Value;
                            }
                        }
                        // to the next dates
                        currentTime = periodEndDate.AddDays(1);
                        periodEndDate = currentTime.AddYears(1);
                    }
                }
            });
            Console.WriteLine(results.Count);
        }
    }

    public interface IMainViewModel : IHasViewService, IHasDataAccessFactory<BacktestDataAccess>
    {
    }
}