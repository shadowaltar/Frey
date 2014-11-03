using Caliburn.Micro;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Trading.Backtest.Data;
using Trading.Common;
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
            StartYears = new BindableCollection<int>(Enumerable.Range(2000, 15));
            EndYears = new BindableCollection<int>(Enumerable.Range(2000, 15));
            SelectedStartYear = 2004;
            SelectedEndYear = 2005;
            if (!Directory.Exists(Constants.LogsDirectory))
            {
                Directory.CreateDirectory(Constants.LogsDirectory);
            }
        }

        public async void Run()
        {
            var d = new DateTime(SelectedStartYear, 1, 1).AddDays(-1).Next(DayOfWeek.Tuesday);

            while (d <= testEnd || d <= endOfData)
            {
                if (core.Positions.Count != 0)
                    core.ExitPositions(d);
                core.EnterPositions(d);
                d.Next(DayOfWeek.Tuesday);
            }
            core.ExitPositions(endOfData);
        }
    }

    public interface IMainViewModel : IHasViewService, IHasDataAccessFactory<BacktestDataAccess>
    {
    }
}