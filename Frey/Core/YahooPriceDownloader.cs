using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Automata.Core
{
    static class YahooStockPriceDownloader
    {
        public static string GetUrl(string code)
        {
            int day = DateTime.Now.Day;
            int month = DateTime.Now.Month - 1;
            int year = DateTime.Now.Year;
            return string.Format("http://ichart.finance.yahoo.com/table.csv?s=SLY&a=0&b=1&c=1950&d={0}&e={1}&f={2}&g=d&ignore=.csv", month, day, year);
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
            }
            catch
            {
                Console.WriteLine("Cannot download {0}!", code);
            }
        }
    }
}
