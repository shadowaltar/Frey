using System;
using System.Collections.Generic;
using System.Diagnostics;
using Caliburn.Micro;
using System.Linq;
using Trading.Backtest.Data;
using Trading.Backtest.Reporting;
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
            SecurityToCheckPrices = new BindableCollection<PriceReportEntry>();

            // set defaults
            SelectedStartYear = 2004;
            SelectedEndYear = 2004;
            SecurityCodeToCheck = "AAPL";
            DayRangeToCheck = 7;
            DateToCheck = 20100101;
        }

        public void Run()
        {
            core.Initialize(testStart, testEnd, endOfData);
            core.Run();

            var reporter = new BacktestReport();
            string path;
            using (ReportTime.Start())
                path = reporter.Create(core);
            Process.Start(path);
        }

        public void TestExcel()
        {
        }

        public void GetSecurityInfoWithDateRange()
        {
            List<PriceReportEntry> x;
            using (var access = DataAccessFactory.New())
            {
                var d = DateToCheck.ConvertDate("yyyyMMdd");
                x = access.YieldQuery(r => new PriceReportEntry
                {
                    At = r["Time"].ConvertDate("yyyyMMdd"),
                    Open = r["Open"].ConvertDouble(),
                    High = r["High"].ConvertDouble(),
                    Low = r["Low"].ConvertDouble(),
                    Close = r["Close"].ConvertDouble(),
                    AdjClose = r["AdjClose"].ConvertDouble(),
                    Volume = r["Volume"].ConvertLong(),
                }, @"SELECT P.* FROM PRICES P JOIN SECURITIES S ON P.SECID = S.ID
WHERE P.TIME >= {0} AND P.TIME <= {1} AND S.CODE = '{2}'", d.AddDays(-DayRangeToCheck).ToDateInt(),
                                                         d.AddDays(DayRangeToCheck).ToDateInt(),
                                                         SecurityCodeToCheck).ToList();
            }
            try
            {
                x.Sort((p1, p2) => p1.At.CompareTo(p2.At));
                var e = x[0];
                for (int i = 1; i < x.Count; i++)
                {
                    var entry = x[i];
                    entry.Return = (entry.AdjClose - e.AdjClose) / entry.AdjClose;
                }
            }
            catch (Exception)
            {
                Log.Error("Cannot prepare the price report entries");
                SecurityToCheckPrices.Clear();
            }
            SecurityToCheckPrices.ClearAndAddRange(x);
        }
    }

    public interface IMainViewModel : IHasViewService, IHasDataAccessFactory<BacktestDataAccess>
    {
    }
}