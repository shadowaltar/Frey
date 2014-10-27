using System;
using System.Collections.Generic;
using Trading.Common.Data;
using Trading.Common.Entities;
using Trading.Common.SharedSettings;
using Trading.Common.Utils;
using Trading.Common.ViewModels;

namespace Trading.Backtest.ViewModels
{
    public partial class MainViewModel : MainViewModelBase<TradingDataAccess>, IMainViewModel
    {
        public MainViewModel(IDataAccessFactory<TradingDataAccess> dataAccessFactory, ISettings settings)
            : base(dataAccessFactory, settings)
        {
            DataAccessFactory = dataAccessFactory;
        }

        public override string ProgramName
        {
            get { return "Backtest"; }
        }

        public void Test()
        {
            var startTime = new DateTime(2004, 1, 1);
            var endTime = new DateTime(2014, 1, 1);
            var currentTime = startTime;
            // day looping
            using (var access = DataAccessFactory.New())
            using (ReportTime.Start("Total test time: {0}"))
            {
                while (currentTime < endTime)
                {
                    List<Security> securities = null;
                    if (currentTime.DayOfWeek == DayOfWeek.Monday)
                    {
                        securities = PrepareSecurities(currentTime);
                    }

                    if (securities != null)
                    {

                    }

                    // to the next date
                    if (currentTime.DayOfWeek == DayOfWeek.Friday)
                        currentTime = currentTime.AddDays(3);
                    else
                        currentTime = currentTime.AddDays(1);
                }
            }

        }
    }

    public interface IMainViewModel : IHasViewService, IHasDataAccessFactory<TradingDataAccess>
    {
    }
}