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
            sheet.SetValue(1, 3, "S&P 500 Normalized");
            sheet.SetValue(1, 4, "S&P 500");
            var firstSpyVal = core.PortfolioStatuses[0].Benchmark;
            for (int i = 0; i < core.PortfolioStatuses.Count; i++)
            {
                var status = core.PortfolioStatuses[i];
                sheet.SetValue(i + marginTop, 1, status.Time.IsoFormat());
                sheet.SetValue(i + marginTop, 2, status.Equity);
                sheet.SetValue(i + marginTop, 3, status.Benchmark / firstSpyVal * core.InitialPortfolioEquity);
                sheet.SetValue(i + marginTop, 4, status.Benchmark);
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
            }

            for (int j = 0; j < trades.Count; j++)
            {
                var trade = trades[j];
                var cols = props.Length;
                for (int i = 1; i <= cols; i++)
                {
                    var info = props[i - 1];
                    if (info.Name == "Parameters")
                    {
                        var parameters = (double[])info.GetValue(trade, null);
                        string str = parameters.Aggregate("", (current, parameter) => current + (parameter + "|"));
                        sheet.SetValue(j + 2, i, str.Trim('|'));
                    }
                    else
                    {
                        sheet.SetValue(j + 2, i, info.GetValue(trade, null));
                    }
                }
            }
            sheet.View.FreezePanes(2, 2);
            xls.Save();
            return resultPath;
        }
    }
}