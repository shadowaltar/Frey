using System.IO;
using System.Net;

namespace Automata.Core
{
    public static class NasdaqDownloader
    {
        public static void DownloadListed(string destinationFolder)
        {
            try
            {
                const string fileName = "Meta_ExchangeTradables_NASDAQ.csv";
                var filePath = Path.Combine(destinationFolder, fileName);
                using (var wc = new WebClient())
                {
                    wc.DownloadFile("ftp://ftp.nasdaqtrader.com/SymbolDirectory/nasdaqlisted.txt",
                        filePath);
                }
                if (File.Exists(filePath))
                {
                    var fileSize = new FileInfo(filePath).Length;
                    var fileSizeFormatted = fileSize > 1024
                        ? (fileSize / 1024).ToString("##.000") + "KBytes"
                        : fileSize + "Bytes";
                    Utilities.WriteTimedLine("File for NASDAQ Listed is downloaded, the size is: {0}", fileSizeFormatted);
                }
                else
                {
                    Utilities.WriteTimedLine("Cannot download NASDAQ Listed!");
                }
            }
            catch
            {
                Utilities.WriteTimedLine("Cannot download NASDAQ Listed!");
            }
        }
    }
}