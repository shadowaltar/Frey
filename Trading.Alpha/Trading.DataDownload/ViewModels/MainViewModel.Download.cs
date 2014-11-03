using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using CsvHelper;
using Trading.Common;
using Trading.Common.Utils;

namespace Trading.DataDownload.ViewModels
{
    public partial class MainViewModel
    {
        List<string> failedAndInvalids = new List<string>();
        int failedAndInvalidCount = 0;

        private string selectedMarket;
        public string SelectedMarket
        {
            get { return selectedMarket; }
            set { SetNotify(ref selectedMarket, value); }
        }

        public BindableCollection<string> Markets { get; private set; }

        private void Initalize()
        {
            Markets = new BindableCollection<string>();
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

                            if (symbol.Contains("^"))
                            {
                                failedAndInvalidCount++;
                                failedAndInvalids.Add(symbol);
                                continue;
                            }

                            results.Add(symbol);
                        }
                        catch (Exception e)
                        {
                            Log.Warn("Failed to read symbol.", e);
                            failedAndInvalidCount++;
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
            var start = DateTime.Parse("1900-01-01");
            Download(start);
        }

        public async void Download(DateTime start)
        {
            if (SelectedMarket.IsNullOrWhitespace())
                return;

            failedAndInvalidCount = 0;
            failedAndInvalids.Clear();

            var results = await ReadSecurityListFile(SelectedMarket);
            if (failedAndInvalidCount > 0)
                await ViewService.ShowError("Some are failed (count: " + failedAndInvalidCount + ")");

            Console.WriteLine("---- Invalid symbols: ");
            foreach (var failedAndInvalid in failedAndInvalids)
            {
                Console.WriteLine(failedAndInvalid);
            }
            Console.WriteLine("---- Invalid symbols END ");

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
                    var total = results.Count();
                    foreach (var symbol in results)
                    {
                        var filePath = symbol.Trim().Replace('/', '[').Replace('\\', ']');
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
}