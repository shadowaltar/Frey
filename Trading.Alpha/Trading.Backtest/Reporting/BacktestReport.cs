using System;
using System.IO;
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
            var sheet = xls.Sheet("Summary");
            sheet.SetKeyAndValuesVertical(0, 0);

            return resultPath;
        }
    }
}