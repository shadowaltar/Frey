using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using Trading.Common;
using Trading.Common.Data;
using Trading.Common.Entities;
using Trading.Common.SharedSettings;
using Trading.Common.Utils;
using Trading.Common.ViewModels;

namespace Trading.WorkExtractor.ViewModels
{
    public class MainViewModel : MainViewModelBase<TradingDataAccess>, IMainViewModel
    {
        public MainViewModel(IDataAccessFactory<TradingDataAccess> dataAccessFactory, ISettings settings)
            : base(dataAccessFactory, settings)
        {
            Constants.InitializeDirectories();
        }

        public override string ProgramName
        {
            get { return "SQL Extraction Generator"; }
        }

        private string resultSqls;
        public string ResultSqls
        {
            get { return resultSqls; }
            set { SetNotify(ref resultSqls, value); }
        }

        public void TestDateTimeVsInteger()
        {
            // prepare
            var dates = new List<DateTime>();
            var doubles = new List<double>();
            var refDate = new DateTime(2000, 1, 1);
            for (int i = 0; i < 1000000; i++)
            {
                var r = StaticRandom.Instance.Next(-20000, 20000);
                var d = refDate.AddDays(r).AddSeconds(StaticRandom.Instance.Next(-20000, 20000));
                dates.Add(d);
                doubles.Add(d.ToTimeDouble());
            }

            // test
            using (ReportTime.Start())
            {
                dates.Sort();
            }
            using (ReportTime.Start())
            {
                doubles.Sort();
            }

            // datetime vs int 160:100; vs double 160:130
        }

        public void GenerateIndustrySql()
        {
            var symbols = new List<string>();
            var files = Directory.GetFiles(Constants.SecurityListDirectory);
            foreach (var symbolFile in files)
            {
                try
                {
                    using (var reader = File.OpenText(symbolFile))
                    using (var records = new CsvReader(reader))
                    {
                        while (records.Read())
                        {
                            var symbol = records.GetField<string>("Symbol");
                            if (string.IsNullOrWhiteSpace(symbol))
                                continue;
                            if (symbol.Contains('^'))
                                continue;

                            symbols.Add(symbol);
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error("Failed to read file " + symbolFile, e);
                }
            }

            var sb = new StringBuilder();
            foreach (var symbol in symbols)
            {
                
            }
        }
    }

    public interface IMainViewModel : IHasViewService
    {
    }
}