using System;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using Trading.Backtest.Data;
using Trading.Backtest.ViewModels;
using Trading.Common;
using Trading.Common.Reporting;
using Trading.Common.Utils;

namespace Trading.Backtest.Reporting
{
    public class BacktestReport
    {
        public string Create(Core core)
        {
            var resultPath = Path.Combine(Constants.LogsDirectory,
                "BacktestReport[" + DateTime.Now.ToTimeDouble() + "].xlsx");
            var xls = new ExcelPackage(new FileInfo(resultPath));
            var sheet = xls.Sheet("Portfolio");

            var marginTop = 2;
            sheet.SetValue(1, 1, "Time");
            sheet.SetValue(1, 2, "Equity");
            for (int i = 0; i < core.PortfolioStatuses.Count; i++)
            {
                var status = core.PortfolioStatuses[i];
                sheet.SetValue(i + marginTop, 1, status.Time.IsoFormat());
                sheet.SetValue(i + marginTop, 2, status.Equity);
            }

            sheet = xls.Sheet("Trades");
            var props = typeof(TradeReportEntry).GetProperties();
            var trades = core.Trades.Select(t => new TradeReportEntry(t)).ToList();

            // headers
            for (int i = 0; i < props.Length; i++)
            {
                var info = props[i];
                sheet.SetValue(1, i + 1, info.Name);
                var t = info.PropertyType;
                if (t == typeof(DateTime))
                {
                    sheet.Column(i).Style.Numberformat.Format = "yyyymmdd";
                }
                //else if (t == typeof(double))
                //{
                //    sheet.Column(i).Style.Numberformat.Format = "0.0000";
                //}
            }

            for (int j = 0; j < trades.Count; j++)
            {
                var trade = trades[j];
                var cols = props.Length;
                for (int i = 1; i <= cols; i++)
                {
                    var info = props[i - 1];
                    sheet.SetValue(j + 2, i, info.GetValue(trade, null));
                }

                var id = DataCache.SecurityCodeMap[trade.SecurityCode];
                var historicalPrices = core.PositionPriceHistory[id];
                var d = trade.EnterTime.ConvertDate("yyyyMMdd");
                d = d.AddDays(-7);
                for (int k = 0; k < 14; k++) // two weeks 
                {
                    double val;
                    if (historicalPrices.TryGetValue(d, out val))
                    {
                        sheet.SetValue(j + 2, k + cols + 1, val);
                    }
                    d = d.AddDays(1);
                }
            }
            sheet.View.FreezePanes(2, 2);
            xls.Save();
            return resultPath;
        }
    }
}