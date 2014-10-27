using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        private static string DefaultSchema { get { return "TRADING"; } }

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
            using (ReportTime.Start("Initialize in-memory database schema used {0}."))
                InitializeDatabase();

            using (ReportTime.Start("Initialize in-memory database data used {0}."))
                InitializeData();
        }

        public void AddUniqueIndexToPrices()
        {
            try
            {
                using (var access = DataAccessFactory.New())
                {
                    access.CreatePriceTableConstraint();
                }
            }
            catch (Exception e)
            {
                Log.Error("Failed to add contraint.", e);
            }
        }

        protected void InitializeDatabase()
        {
            using (var access = DataAccessFactory.New())
            {
                if (!access.CheckSchemaExists(DefaultSchema))
                {
                    access.CreateSchema(DefaultSchema);
                }
                access.UseSchema(DefaultSchema);
                access.CreateSecurityTable();
                access.CreateSecurityPriceTable();
            }
        }

        protected async void InitializeData()
        {
            var p = await ViewService.ShowProgress("Loading", "Reading security files..");
            await Task.Run(() =>
            {
                var files = Directory.GetFiles(Constants.SecurityListDirectory);
                using (var access = DataAccessFactory.New())
                {
                    access.Clear("SECURITIES");
                }
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
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
            });
            await Task.Run(() =>
            {
                var folders = Directory.GetDirectories(Constants.PricesDirectory);
                Dictionary<string, Security> securities;
                using (var access = DataAccessFactory.New())
                {
                    securities = access.GetSecurities().ToDictionary(s => s.Code, s => s);
                    access.Clear("PRICES");
                }
                foreach (var folder in folders)
                {
                    using (ReportTime.Start(folder + " prices loaded used: {0}"))
                    {
                        try
                        {
                            using (var access = DataAccessFactory.NewTransaction())
                            using (new ReportTime("Read " + folder + " used {0}"))
                            {
                                var files = Directory.GetFiles(folder);
                                double cnt = 0;
                                var n = files.Count();

                                foreach (var fileName in files)
                                {
                                    try
                                    {
                                        p.SetMessage("Reading " + fileName + " (" + (cnt / n).ToString("P") + ")");
                                        var code = Path.GetFileNameWithoutExtension(fileName);
                                        long secId;
                                        Security sec;
                                        if (!securities.TryGetValue(code, out sec))
                                        {
                                            Log.WarnFormat("Cannot handle code {0}", code);
                                            continue;
                                        }
                                        secId = sec.Id;

                                        if (!securities.ContainsKey(code.ToUpperInvariant()))
                                            continue;
                                        using (var reader = File.OpenText(fileName))
                                        using (var records = new CsvReader(reader))
                                        {
                                            while (records.Read())
                                            {
                                                try
                                                {
                                                    var time = records.GetField<string>("Date")
                                                        .ConvertDate("yyyy-MM-dd");
                                                    var open = records.GetField<double>("Open");
                                                    var high = records.GetField<double>("High");
                                                    var low = records.GetField<double>("Low");
                                                    var close = records.GetField<double>("Close");
                                                    var volume = records.GetField<double>("Volume");
                                                    var adjClose = records.GetField<double>("Adj Close");
                                                    access.AddPrice(secId, time, open, high, low, close, volume,
                                                        adjClose);
                                                }
                                                catch (Exception e)
                                                {
                                                    Log.Warn("Failed to read symbol.", e);
                                                }
                                            }
                                        }
                                        cnt++;
                                        Console.WriteLine(cnt / n);
                                    }
                                    catch (Exception e)
                                    {
                                        Log.Error("Failed to handle file " + fileName, e);
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Log.Error("Error occurred when inserting prices: folder " + folder, e);
                        }
                    }
                }
            });
            await p.Stop();
        }
    }

    public interface IMainViewModel : IHasViewService, IHasDataAccessFactory<CacheDataAccess>
    {
    }
}