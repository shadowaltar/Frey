using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using Trading.Common;
using Trading.Common.Data;
using Trading.Common.Entities;
using Trading.Common.SharedSettings;
using Trading.Common.Utils;
using Trading.Common.ViewModels;

namespace Trading.DataDownload.ViewModels
{
    public partial class MainViewModel : MainViewModelBase<LoaderDataAccess>, IMainViewModel
    {
        public MainViewModel(IDataAccessFactory<LoaderDataAccess> dataAccessFactory, ISettings settings)
            : base(dataAccessFactory, settings)
        {
            Constants.InitializeDirectories();
            Initalize();
        }

        public override string ProgramName { get { return "Data Loader"; } }

        private string selectedStockCode;
        private string selectedMarketCode;

        public string SelectedStockCode
        {
            get { return selectedStockCode; }
            set { SetNotify(ref selectedStockCode, value); }
        }

        public string SelectedMarketCode
        {
            get { return selectedMarketCode; }
            set { SetNotify(ref selectedMarketCode, value); }
        }

        public async void DownloadSingleStockPrice()
        {
            var prog = await ViewService.ShowProgress("Loading", "Parsing / saving " + SelectedMarketCode + " security information...");
            prog.AppendMessageForLoading("Saving securities...");
            await Task.Run(() => SaveStocks(SelectedMarketCode, new Stock { Code = SelectedStockCode }));

            prog.AppendMessageForLoading("Downloading securities prices...");
            await Task.Run(() => Download(SelectedStockCode, SelectedMarketCode));
            await prog.Stop();
        }

        public void Download(string symbol, string marketCode)
        {
            var downloader = new YahooDailyPriceWorker();
            var dir = Path.Combine(Constants.PricesDirectory, marketCode);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var pathSymbol = symbol.Trim();
            if (pathSymbol.Contains('/'))
                pathSymbol = pathSymbol.Replace('/', '[');
            if (pathSymbol.Contains('\\'))
                pathSymbol = pathSymbol.Replace('\\', ']');
            if (pathSymbol.Contains('^'))
                pathSymbol = pathSymbol.Replace("^", "%5E");
            downloader.Download(symbol, Path.Combine(dir, pathSymbol + ".csv"));
        }

        public void OpenPricesFolder()
        {
            var p = new Process();
            p.StartInfo.FileName = Constants.PricesDirectory;
            p.Start();
        }

        public void OpenSecurityListFolder()
        {
            var p = new Process();
            p.StartInfo.FileName = Constants.SecurityListDirectory;
            p.Start();
        }

        public async void DownloadAllMarkets()
        {
            await DownloadPricesForMarket("INDEX");
            await DownloadPricesForMarket("AMEX");
            await DownloadPricesForMarket("NYSE");
            await DownloadPricesForMarket("NASDAQ");
        }

        public async void DownloadAllAmex()
        {
            await DownloadPricesForMarket("AMEX");
        }

        public async void DownloadAllNyse()
        {
            await DownloadPricesForMarket("NYSE");
        }

        public async void DownloadAllNasdaq()
        {
            await DownloadPricesForMarket("NASDAQ");
        }

        public async void DownloadAllNyseSectorIndexes()
        {
            await DownloadPricesForMarket("INDEX");
        }

        public async Task DownloadPricesForMarket(string marketCode)
        {
            var progressCount = 0;
            var totalCount = 0d;
            var tasks = new List<Task>();
            var prog = await ViewService.ShowProgress("Loading", "Parsing / saving " + marketCode + " security information...");
            try
            {
                await Task.Run(() =>
                {
                    var sectorIndustries = new HashSet<Tuple<string, string>>();
                    var securities = new List<Stock>();
                    var files = Directory.GetFiles(Constants.SecurityListDirectory);
                    var symbolFile = files.FirstOrDefault(f => f.ContainsIgnoreCase(marketCode));
                    if (symbolFile != null)
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

                                var name = records.GetField<string>("Name");
                                var sector = records.GetField<string>("Sector");
                                var industry = records.GetField<string>("industry");
                                sectorIndustries.Add(new Tuple<string, string>(sector, industry));
                                securities.Add(new Stock { Code = symbol, Name = name, Sector = sector, Industry = industry });
                            }
                        }

                        prog.AppendMessageForLoading("Saving sectors and industries...");
                        using (var access = DataAccessFactory.NewTransaction())
                        {
                            try
                            {
                                access.DeleteSectorIndustries(marketCode);
                                using (new ReportTime())
                                {
                                    foreach (var si in sectorIndustries)
                                    {
                                        if (!access.CreateSectorIndustry(marketCode, si.Item1, si.Item2))
                                        {
                                            access.Rollback();
                                        }
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                access.Rollback();
                                throw;
                            }
                        }
                        prog.AppendMessageForLoading("Saving securities...");
                        SaveStocks(marketCode, securities.ToArray());

                        prog.AppendMessageForLoading("Downloading securities prices...");
                        totalCount = securities.Count;

                        foreach (var security in securities)
                        {
                            Stock security1 = security;
                            tasks.Add(Task.Run(() =>
                            {
                                Download(security1.Code, marketCode);
                                Interlocked.Increment(ref progressCount);
                                prog.AppendMessageForLoading(progressCount + "/" + totalCount + " " + security1.Code);
                                prog.SetProgress(progressCount / totalCount);
                            }));
                        }
                    }
                });
            }
            catch (Exception e)
            {
                Log.Debug("!", e);
            }
            await Task.WhenAll(tasks.ToArray());
            await prog.Stop();
        }

        private void SaveStocks(string marketCode, params Stock[] securities)
        {
            using (var access = DataAccessFactory.NewTransaction())
            {
                try
                {
                    using (new ReportTime())
                    {
                        var midTable = access.GetMarketId(marketCode);
                        var mid = (int)midTable.Rows[0][0];
                        access.DeleteSecurities(mid);
                        foreach (var security in securities)
                        {
                            access.DeleteSecurity(security.Code, mid);
                            var rowCount = access.CreateStock(security, marketCode, mid);
                            if (rowCount == 0)
                            {
                                access.Rollback();
                                break;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    access.Rollback();
                    throw;
                }
            }
        }

        public void SaveNasdaqPriceToDatabase()
        {
            SavePriceToDatabase("NASDAQ");
        }

        public void SaveAmexPriceToDatabase()
        {
            SavePriceToDatabase("AMEX");
        }

        public void SaveNysePriceToDatabase()
        {
            SavePriceToDatabase("NYSE");
        }

        public async void SavePriceToDatabase(string marketCode)
        {
            var prog = await ViewService.ShowProgress("Saving", "Saving prices for market " + marketCode);
            try
            {
                var mktDir = Path.Combine(Constants.PricesDirectory, marketCode);
                var files = Directory.GetFiles(mktDir);
                var totalCount = files.Length.ConvertDouble();
                var progressCount = 0;
                await Task.Run(() =>
                {
                    using (new ReportTime())
                    {
                        bool isGood = true;
                        foreach (var file in files)
                        {
                            if (!isGood)
                                break;
                            var securityCode = Path.GetFileNameWithoutExtension(file);
                            prog.AppendMessageForLoading("Inserting price for " + securityCode + " " +
                                                                   progressCount + "/" + totalCount);
                            using (var reader = File.OpenText(file))
                            using (var records = new CsvReader(reader))
                            using (var access = DataAccessFactory.NewTransaction())
                            {
                                var sid = access.GetSecurityId(securityCode, marketCode);
                                if (sid == -1)
                                {
                                    Log.WarnFormat("Cannot find {0} (MKT:{1}) from db.", securityCode, marketCode);
                                    continue;
                                }
                                access.DeleteSecurityPrices(sid);
                                int rowCount = 0;
                                Interlocked.Increment(ref progressCount);
                                while (records.Read())
                                {
                                    var date = records.GetField<string>("Date").ConvertIsoDate();
                                    var open = records.GetField<decimal>("Open");
                                    var high = records.GetField<decimal>("High");
                                    var low = records.GetField<decimal>("Low");
                                    var close = records.GetField<decimal>("Close");
                                    var volume = records.GetField<decimal>("Volume");
                                    var adjClose = records.GetField<decimal>("Adj Close");
                                    if (!access.InsertPrice(sid, date, open, high, low, close, volume, adjClose))
                                    {
                                        access.Rollback();
                                        isGood = false;
                                        break;
                                    }
                                    rowCount++;
                                    if (rowCount % 200 == 0)
                                    {
                                        prog.AppendMessageForLoading("Inserting price for " + securityCode + " " +
                                                                   progressCount + "/" + totalCount + " (" + rowCount / 500 * 500 + ")");
                                    }
                                }
                                prog.SetProgress(progressCount / totalCount);
                            }
                        }
                    }
                });
            }
            catch (Exception e)
            {
                Log.Debug("!", e);
            }
            await prog.Stop();
        }
    }

    public interface IMainViewModel : IHasViewService
    {
    }
}