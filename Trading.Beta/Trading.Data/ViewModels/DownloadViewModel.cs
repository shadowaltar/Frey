using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Caliburn.Micro;
using CsvHelper;
using Trading.Common;
using Trading.Common.Utils;
using Trading.Common.ViewModels;
using Trading.Data.Data;

namespace Trading.Data.ViewModels
{
    public class DownloadViewModel : ViewModelBase, IDownloadViewModel
    {
        protected DateTime DataStart { get { return new DateTime(1999, 1, 1); } }

        private readonly BindableCollection<string> markets = new BindableCollection<string>();
        public BindableCollection<string> Markets { get { return markets; } }

        private string selectedMarket;
        public string SelectedMarket { get { return selectedMarket; } set { SetNotify(ref selectedMarket, value); } }

        private string singleDownloadCode;
        public string SingleDownloadCode { get { return singleDownloadCode; } set { SetNotify(ref singleDownloadCode, value); } }

        public IViewService ViewService { get; set; }

        public void Initalize()
        {
            var files = Directory.GetFiles(Constants.SecurityListDirectory);
            foreach (var file in files)
            {
                Markets.Add(Path.GetFileNameWithoutExtension(file));
            }
        }

        public async Task<List<string>> ReadSecurityListFile(string market)
        {
            if (market.IsNullOrWhitespace())
                return null;

            var fileName = market + ".csv";
            var filePath = Path.Combine(Constants.SecurityListDirectory, fileName);
            if (filePath.IsNullOrWhitespace())
                return null;

            var isGood = true;
            var results = new List<string>();
            try
            {
                using (new ReportTime("Read " + fileName + " used {0}"))
                using (var reader = File.OpenText(filePath))
                using (var records = new CsvReader(reader))
                {
                    while (records.Read())
                    {
                        try
                        {
                            var symbol = records.GetField<string>("Symbol");
                            if (string.IsNullOrWhiteSpace(symbol))
                                continue;
                            results.Add(symbol);
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
                isGood = false;
            }
            if (!isGood)
                await ViewService.ShowError("Error when reading symbols for market: " + market);
            return results;
        }

        public void DownloadWholeHistory()
        {
            var start = DataStart;
            Download(start);
        }

        public async void DownloadByCode()
        {
            SelectedMarket = null;
            var mkt = "US";
            var start = DataStart;

            var dir = Path.Combine(Constants.PricesDirectory, mkt);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            try
            {
                var p = await ViewService.ShowProgress("Downloading", "MARKET: " + mkt);
                await Task.Run(() =>
                {
                    var downloader = new YahooDailyPriceWorker();
                    var filePath = SingleDownloadCode.Trim().Replace('/', '[').Replace('\\', ']').Replace(" ", "-");
                    filePath = Path.Combine(dir, filePath + ".csv");

                    using (var rt = ReportTime.Start(string.Format("Download {0}", SingleDownloadCode) + " used {0}"))
                    {
                        downloader.Download(SingleDownloadCode, start, filePath);
                        p.AppendMessageForLoading(rt.Message);
                    }
                });
                await p.Stop();
            }
            catch (Exception e)
            {
                Log.Error("Failed to download prices from yahoo for a week where market is " + SelectedMarket, e);
            }
        }

        public async void Download(DateTime start)
        {
            if (SelectedMarket.IsNullOrWhitespace())
                return;

            var results = await ReadSecurityListFile(SelectedMarket);

            var dir = Path.Combine(Constants.PricesDirectory, SelectedMarket);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            try
            {
                var p = await ViewService.ShowProgress("Downloading", "MARKET: " + SelectedMarket);
                await Task.Run(() =>
                {
                    var downloader = new YahooDailyPriceWorker();
                    var cnt = 1;
                    var total = results.Count;
                    foreach (var symbol in results)
                    {
                        var filePath = symbol.Trim().Replace('/', '[').Replace('\\', ']').Replace(" ", "-");
                        filePath = Path.Combine(dir, filePath + ".csv");

                        using (var rt = ReportTime.Start(string.Format("Download {0} ({1}/{2})", symbol, cnt, total)
                                                         + " used {0}"))
                        {
                            downloader.Download(symbol, start, filePath);
                            p.AppendMessageForLoading(rt.Message);
                            p.SetProgress((double)cnt / total);
                        }

                        cnt++;
                    }
                });
                await p.Stop();
            }
            catch (Exception e)
            {
                Log.Error("Failed to download prices from yahoo for a week where market is " + SelectedMarket, e);
            }
        }
    }

    public interface IDownloadViewModel : IHasViewService
    {
        void Initalize();
    }
}