using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Trading.Common;
using Trading.Common.Data;
using Trading.Common.Entities;
using Trading.Common.Utils;
using Trading.Common.ViewModels;
using Trading.Data.Data;

namespace Trading.Data.ViewModels
{
    public class DatabaseViewModel : ViewModelBase, IDatabaseViewModel
    {
        private static string DefaultSchema { get { return "TRADING"; } }

        public IViewService ViewService { get; set; }
        public IDataAccessFactory<Access> DataAccessFactory { get; set; }

        private string priceFilePath;
        public string PriceFilePath
        {
            get { return priceFilePath; }
            set { SetNotify(ref priceFilePath, value); }
        }

        private string priceFileSecurityName;
        public string PriceFileSecurityName
        {
            get { return priceFileSecurityName; }
            set { SetNotify(ref priceFileSecurityName, value); }
        }

        /// <summary>
        /// insert securities and prices
        /// </summary>
        public void CreateAndInsertAllSecuritiesAndPrices()
        {
            using (ReportTime.Start("Initialize in-memory database schema used {0}."))
                InitializeDatabase();

            using (ReportTime.Start("Initialize in-memory database data used {0}."))
                InitializeData();
        }

        public async void InsertSinglePriceFile()
        {
            var p = await ViewService.ShowProgress("Loading", "Reading security files..");
            await Task.Run(() =>
            {
                try
                {
                    var code = Path.GetFileNameWithoutExtension(PriceFilePath);
                    using (var access = DataAccessFactory.NewTransaction())
                    {
                        access.Execute("DELETE P.* FROM PRICES P JOIN SECURITIES S on P.SECID = S.ID WHERE S.CODE = '{0}'", code);
                        access.Execute("DELETE FROM SECURITIES WHERE CODE = '{0}'", code);
                        access.AddSecurity(code, PriceFileSecurityName);
                    }
                    using (var access = DataAccessFactory.NewTransaction())
                    using (new ReportTime("Read " + PriceFilePath + " used {0}"))
                    using (var reader = File.OpenText(PriceFilePath))
                    using (var records = new CsvReader(reader))
                    {
                        Security sec = access.GetSecurity(code);
                        var secId = sec.Id;
                        try
                        {
                            while (records.Read())
                            {
                                try
                                {
                                    var time = records.GetField<string>("Date")
                                        .ConvertDate("yyyy-MM-dd").ToDateInt();
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
                        catch (Exception e)
                        {
                            Log.Warn("Failed to read symbol.", e);
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            });
            await p.Stop();
        }

        public async void InsertIfNotExists()
        {
            Dictionary<string, Security> allSecurities = null;
            var p = await ViewService.ShowProgress("Loading", "Reading security files..");
            await Task.Run(() =>
            {
                var files = Directory.GetFiles(Constants.SecurityListDirectory);
                var missingSecurities = new HashSet<string>();
                using (var access = DataAccessFactory.New())
                {
                    allSecurities = access.GetSecurities().ToDictionary(s => s.Code, s => s);
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
                                    if (allSecurities.ContainsKey(symbol))
                                        continue;

                                    access.AddSecurity(symbol, name);
                                    missingSecurities.Add(symbol);
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
                using (var access = DataAccessFactory.New())
                {
                    allSecurities = access.GetSecurities().ToDictionary(s => s.Code, s => s);
                }
            });

            if (allSecurities == null)
            {
                await ViewService.ShowError("Cannot add prices as security list is not ready.");
                return;
            }

            await Task.Run(() =>
            {
                var folders = Directory.GetDirectories(Constants.PricesDirectory);
                foreach (var folder in folders)
                {
                    using (ReportTime.Start(folder + " prices loaded used: {0}"))
                    {
                        using (var access = DataAccessFactory.NewTransaction())
                        using (new ReportTime("Read " + folder + " used {0}"))
                        {
                            try
                            {
                                var files = Directory.GetFiles(folder);
                                double cnt = 0;
                                var n = files.Count();

                                foreach (var fileName in files)
                                {
                                    var code = Path.GetFileNameWithoutExtension(fileName);

                                    if (!allSecurities.ContainsKey(code))
                                    {
                                        Console.WriteLine("Failed insert price for file: " + fileName);
                                        continue;
                                    }

                                    var sid = allSecurities[code].Id;
                                    var t = access.Query("SELECT MAX(TIME) FROM PRICES WHERE SECID = {0}", sid).FirstOrDefaultValue<int>();
                                    p.SetMessage("Reading " + fileName + " (" + (cnt / n).ToString("P") + ")");

                                    using (var reader = File.OpenText(fileName))
                                    using (var records = new CsvReader(reader))
                                    {
                                        while (records.Read())
                                        {
                                            var time = records.GetField<string>("Date")
                                                .ConvertDate("yyyy-MM-dd").ToDateInt();

                                            if (time <= t) // already inserted this date.
                                                break; // all the other rows must be smaller, so break.

                                            var open = records.GetField<double>("Open");
                                            var high = records.GetField<double>("High");
                                            var low = records.GetField<double>("Low");
                                            var close = records.GetField<double>("Close");
                                            var volume = records.GetField<double>("Volume");
                                            var adjClose = records.GetField<double>("Adj Close");
                                            access.AddPrice(sid, time, open, high, low, close, volume, adjClose);
                                        }
                                    }
                                    cnt++;
                                    Console.WriteLine(cnt / n);
                                }
                            }
                            catch (Exception e)
                            {
                                Log.Error("Error occurred when inserting prices: folder " + folder, e);
                                access.Rollback();
                            }
                        }

                    }
                }
            });
            await p.Stop();
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

        public void AddCalendar()
        {
            using (var access = DataAccessFactory.NewTransaction())
            {
                try
                {
                    access.UseSchema(DefaultSchema);
                    access.CreateCalendarTable();

                    var fileName = Directory.GetFiles(Constants.OtherDataDirectory, "US_CALENDAR.csv", SearchOption.TopDirectoryOnly)
                        .FirstOrDefault();
                    using (new ReportTime("Read " + fileName + " used {0}"))
                    using (var reader = File.OpenText(fileName))
                    using (var records = new CsvReader(reader))
                    {
                        while (records.Read())
                        {
                            try
                            {
                                var ctry = records.GetField<string>("COUNTRY");
                                var date = records.GetField<string>("DATE").ConvertIsoDate().ToDateInt();
                                var holiday = records.GetField<int>("HOLIDAY");
                                var mktClose = records.GetField<int>("MARKETCLOSE");
                                access.AddCalendar(ctry, date, holiday, mktClose);
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
                    Log.Error("Failed to add CALENDAR.", e);
                    access.Rollback();
                }
            }
        }

        public void AddCountries()
        {
            using (var access = DataAccessFactory.NewTransaction())
            {
                try
                {
                    access.UseSchema(DefaultSchema);
                    access.CreateCountryTable();

                    var fileName = Directory.GetFiles(Constants.OtherDataDirectory, "COUNTRIES.csv", SearchOption.TopDirectoryOnly)
                        .FirstOrDefault();
                    using (new ReportTime("Read " + fileName + " used {0}"))
                    using (var reader = File.OpenText(fileName))
                    using (var records = new CsvReader(reader))
                    {
                        while (records.Read())
                        {
                            try
                            {
                                var code = records.GetField<string>("Code");
                                var name = records.GetField<string>("Name");
                                access.AddCountry(code, name);
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
                    Log.Error("Failed to add CALENDAR.", e);
                    access.Rollback();
                }
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
                                        if (code == null)
                                            throw new InvalidOperationException("invalid code from filename " + fileName);

                                        Security sec;
                                        if (!securities.TryGetValue(code, out sec))
                                        {
                                            Log.WarnFormat("Cannot handle code {0}", code);
                                            continue;
                                        }
                                        var secId = sec.Id;

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
                                                        .ConvertDate("yyyy-MM-dd").ToDateInt();
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

    public interface IDatabaseViewModel : IHasViewService, IHasDataAccessFactory<Access>
    {
    }
}