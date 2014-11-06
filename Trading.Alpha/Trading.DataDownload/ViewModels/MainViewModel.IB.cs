using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Trading.Common;
using Trading.Common.Utils;

namespace Trading.DataDownload.ViewModels
{
    public partial class MainViewModel
    {
        public async void ParseNonShortableSecurityListInteractiveBrokers()
        {
            var p = await ViewService.ShowProgress("Loading", "Requesting from IB...");

            const string signature1 = @"','600','600','custom','front');"">";
            const string signature2 = @"</a>	</td>	<td class='comm_table_content lineRightGray'>";
            const string signature3 = @"	<td class='comm_table_content lineRightGray'>";
            const string signature4 = @"</td>";
            var pairs = new Dictionary<string, string>();
            var code = "";
            var isDownloadGood = true;
            await Task.Run(() =>
            {
                try
                {
                    using (ReportTime.Start("Web request used: {0}"))
                    {
                        var result = Websites.GetString(
                            "https://www.interactivebrokers.com/en/index.php?f=4587&cntry=regSHO&tag=Reg%20SHO&ib_entity=llc&ln=");

                        var lines = result.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        var searchForName = false;
                        foreach (var line in lines)
                        {
                            if (searchForName) // line 2, name
                            {
                                pairs[code] = line.SubstringBetween(signature3, signature4).SimpleUnescape();
                                searchForName = false;
                            }
                            else if (line.Contains(signature1) && line.Contains(signature2)) // line 1, code
                            {
                                code = line.SubstringBetween(signature1, signature2).SimpleUnescape();
                                searchForName = true;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    ViewService.ShowError("Failed to parse!");
                    Log.Error("Failed to parse security list from IB.");
                    isDownloadGood = false;
                }
            });

            if (isDownloadGood)
            {
                p.SetMessage("Saving to file...");
                try
                {
                    var lines = pairs.Select(a => a.Key + "," + a.Value).ToList();
                    lines.Insert(0, "SYMBOL,NAME");
                    File.WriteAllLines(Path.Combine(Constants.SecurityListDirectory, "US_ALL_NonShortable.csv"), lines);
                }
                catch (Exception)
                {
                    ViewService.ShowError("Failed to save!");
                    Log.Error("Failed to parse security list from IB.");
                }
            }
            await p.Stop();
        }

        public async void ParseShortableSecurityListInteractiveBrokers()
        {
            var p = await ViewService.ShowProgress("Loading", "Requesting from IB...");

            const string signature0 = "<tr class='linebottom'>";
            const string signature1 = "	<td class='comm_table_content lineRightGray'>";
            const string signature2 = "	</td>	<td class='comm_table_content lineRightGray'>";
            const string signature3 = "	<td class='comm_table_content lineRightGray'>";
            const string signature4 = "</td>";

            var pairs = new Dictionary<string, string>();
            var code = "";

            var isDownloadGood = true;
            await Task.Run(() =>
            {
                try
                {
                    using (ReportTime.Start("Web request used: {0}"))
                    {
                        var result = Websites.GetString(
                            "https://www.interactivebrokers.com/en/?f=%2Fen%2Ftrading%2FViewShortableStocks.php%3Fcntry%3Dusa%26amp%3Btag%3DUnited%252520States%26amp%3Bib_entity%3Dllc%26amp%3Bln");

                        var lines = result.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        var searchForCode = false;
                        var searchForName = false;
                        foreach (var line in lines)
                        {
                            if (searchForCode)
                            {
                                code = line.SubstringBetween(signature1, signature2).SimpleUnescape();
                                searchForCode = false;
                            }
                            else if (searchForName) // line 2, name
                            {
                                pairs[code] = line.SubstringBetween(signature3, signature4).SimpleUnescape();
                                searchForName = false;
                            }
                            else if (line == signature0) // line 1, code
                            {
                                searchForCode = true;
                                searchForName = true;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    ViewService.ShowError("Failed to parse!");
                    Log.Error("Failed to parse security list from IB.");
                    isDownloadGood = false;
                }
            });

            if (isDownloadGood)
            {
                p.SetMessage("Saving to file...");
                try
                {
                    var lines = pairs.Select(a => a.Key + "," + a.Value).ToList();
                    lines.Insert(0, "SYMBOL,NAME");
                    File.WriteAllLines(Path.Combine(Constants.SecurityListDirectory, "US_ALL_Shortable.csv"), lines);
                }
                catch (Exception)
                {
                    ViewService.ShowError("Failed to save!");
                    Log.Error("Failed to parse security list from IB.");
                }
            }
            await p.Stop();
        }
    }
}