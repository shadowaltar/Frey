using System;
using System.IO;
using System.Linq;
using OfficeOpenXml;
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
            for (int i = 0; i < props.Length; i++)
            {
                var info = props[i];
                sheet.SetValue(1, i + 1, info.Name);
                for (int j = 0; j < trades.Count; j++)
                {
                    var trade = trades[j];
                    sheet.SetValue(j + 2, i + 1, info.GetValue(trade, null));
                }
            }

            xls.Save();
            return resultPath;
        }
    }
}