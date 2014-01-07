using System.IO;
using System.Linq;
using System.Net;

namespace Automata.Core
{
    public static class MiscDownloader
    {
        public static void Download(params string[] urls)
        {
            using (var wc = new WebClient())
            {
                foreach (var url in urls)
                {
                    var fileName = url.Split('/').Last();
                    var filePath = Path.Combine(Context.StaticDataFileDirectory, fileName);

                    System.Console.WriteLine("START "+filePath);
                    wc.DownloadFile(url, filePath);
                    System.Console.WriteLine("DONE");
                }
            }
        }
    }
}