using System;
using System.IO;
using System.Net;

namespace Automata.Core
{
    static class YahooStockPriceDownloader
    {
        public static string GetUrl(string code)
        {
            int day = DateTime.Now.Day;
            int month = DateTime.Now.Month - 1;
            int year = DateTime.Now.Year;
            return string.Format("http://ichart.finance.yahoo.com/table.csv?s={0}&a=0&b=1&c=1980&d={1}&e={2}&f={3}&g=d&ignore=.csv", code, month, day, year);
        }

        public static void Download(string code, string exchange, string destinationFolder)
        {
            try
            {
                var url = GetUrl(code);
                var fileName = exchange + "_" + code + ".csv";
                var filePath = Path.Combine(destinationFolder, fileName);

                // Create a new WebClient instance.
                using (var wc = new WebClient())
                {
                    wc.DownloadFile(url, filePath);
                }
                if (File.Exists(filePath))
                {
                    var fileSize = new FileInfo(filePath).Length;
                    var fileSizeFormatted = fileSize > 1024
                        ? (fileSize / 1024).ToString("##.000") + "KBytes"
                        : fileSize + "Bytes";
                    Utilities.WriteTimedLine("File for {0} is downloaded, the size is: {1}", code, fileSizeFormatted);
                }
                else
                {
                    Utilities.WriteTimedLine("Cannot download {0}!", code);
                }
            }
            catch
            {
                Utilities.WriteTimedLine("Cannot download {0}!", code);
            }
        }
    }
}
