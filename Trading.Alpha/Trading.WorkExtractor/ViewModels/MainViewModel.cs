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
            if (!Directory.Exists(Constants.PricesDirectory))
            {
                Directory.CreateDirectory(Constants.PricesDirectory);
            }
            if (!Directory.Exists(Constants.SecurityListDirectory))
            {
                Directory.CreateDirectory(Constants.SecurityListDirectory);
            }
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