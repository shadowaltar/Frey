using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Web;
using log4net;

namespace Trading.DataDownload
{
    public class YahooDailyPriceWorker
    {
        protected static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private const string DownloadUrl
            = @"http://real-chart.finance.yahoo.com/table.csv?s={0}&a={1}&b={2}&c={3}&d={4}&e={5}&f={6}&g=d&ignore=.csv";

        public void Download(string yahooStockSymbol, DateTime from, DateTime to, string saveFilePath)
        {
            var url = string.Format(DownloadUrl, yahooStockSymbol,
                from.Month - 1, from.Day, from.Year,
                to.Month - 1, to.Day, to.Year);
            var req = WebRequest.Create(url);
            req.Timeout = 20 * 1000;
            try
            {
                using (var response = req.GetResponse())
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        if (responseStream != null)
                        {
                            using (var reader = new StreamReader(responseStream))
                            {
                                var content = reader.ReadToEnd();
                                if (File.Exists(saveFilePath))
                                    File.Delete(saveFilePath);
                                File.WriteAllText(saveFilePath, content);
                                Log.InfoFormat("Saved {0} ({1} - {2}) to {3}", yahooStockSymbol, from, to, saveFilePath);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public void Download(string yahooStockSymbol, DateTime from, string saveFilePath)
        {
            Download(yahooStockSymbol, from, DateTime.Today, saveFilePath);
        }

        public void Download(string yahooStockSymbol, string saveFilePath)
        {
            Download(yahooStockSymbol, DateTime.MinValue, DateTime.Today, saveFilePath);
        }
    }
}