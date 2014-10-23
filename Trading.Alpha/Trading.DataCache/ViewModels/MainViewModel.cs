using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Documents;
using CsvHelper;
using Trading.Common;
using Trading.Common.Data;
using Trading.Common.Entities;
using Trading.Common.SharedSettings;
using Trading.Common.Utils;
using Trading.Common.ViewModels;
using Trading.DataCache.Data;

namespace Trading.DataCache.ViewModels
{
    public class MainViewModel : MainViewModelBase<CacheDataAccess>, IMainViewModel
    {
        private Process dbProcess;

        public MainViewModel(IDataAccessFactory<CacheDataAccess> dataAccessFactory, ISettings settings)
            : base(dataAccessFactory, settings)
        {
            DataAccessFactory = dataAccessFactory;
        }

        public override string ProgramName
        {
            get { return "SQLite Data Cache"; }
        }

        public void RunDatabase()
        {
            using (ReportTime.Start("Start in-memory database used {0}."))
            {
                var result = FileHelper.FindFileInParents("sqlite3.exe");
                dbProcess = Process.Start(result);
            }
            using (ReportTime.Start("Initialize in-memory database schema used {0}."))
                InitializeDatabase();

            using (ReportTime.Start("Initialize in-memory database data used {0}."))
                InitializeData();
        }

        protected void InitializeDatabase()
        {
            using (var access = DataAccessFactory.New())
            {
                access.CreateSecurityTable();
                access.CreateSecurityPriceTable();
            }
        }

        protected async void InitializeData()
        {
            var p = await ViewService.ShowProgress("Loading", "Reading security files..");
            var securitiesTask = Task.Run(() =>
            {
                var files = Directory.GetFiles(Constants.SecurityListDirectory);
                foreach (var fileName in files)
                {
                    try
                    {
                        using (var access = DataAccessFactory.NewTransaction())
                        using (new ReportTime("Read " + fileName + " used {0}"))
                        using (var reader = File.OpenText(fileName))
                        using (var records = new CsvReader(reader))
                        {
                            while (records.Read())
                            {
                                try
                                {
                                    var symbol = records.GetField<string>("Symbol");
                                    var name = records.GetField<string>("Name");
                                    if (string.IsNullOrWhiteSpace(symbol))
                                        continue;

                                    if (symbol.Contains("^"))
                                    {
                                        continue;
                                    }
                                    access.AddSecurity(symbol, name);
                                }
                                catch (Exception e)
                                {
                                    Log.Warn("Failed to read symbol.", e);
                                }
                            }
                        }
                        Console.WriteLine("{0} is done.", fileName);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
            });
            var pricesTask = Task.Run(() =>
            {
                var folders = Directory.GetDirectories(Constants.PricesDirectory);
                foreach (var folder in folders)
                {
                    try
                    {
                        using (var access = DataAccessFactory.NewTransaction())
                        using (new ReportTime("Read " + folder + " used {0}"))
                        {
                            foreach (var fileName in Directory.GetFiles(folder))
                            {
                                using (var reader = File.OpenText(fileName))
                                using (var records = new CsvReader(reader))
                                {
                                    while (records.Read())
                                    {
                                        try
                                        {
                                            var time = records.GetField<string>("Date").ConvertDate("yyyy-MM-dd");
                                            var open = records.GetField<double>("Open");
                                            var high = records.GetField<double>("High");
                                            var low = records.GetField<double>("Low");
                                            var close = records.GetField<double>("Close");
                                            var volume = records.GetField<double>("Volume");
                                            var adjClose = records.GetField<double>("Adj Close");
                                        }
                                        catch (Exception e)
                                        {
                                            Log.Warn("Failed to read symbol.", e);
                                        }
                                    }
                                }
                                Console.WriteLine("{0} is done.", fileName);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
            });


            await securitiesTask;
            await pricesTask;
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            if (dbProcess != null)
                dbProcess.Kill();
        }
    }

    public interface IMainViewModel : IHasViewService, IHasDataAccessFactory<CacheDataAccess>
    {
    }
}