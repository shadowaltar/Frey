using log4net;
using System;
using System.Diagnostics;

namespace Maintenance.Common.Utils
{
    public class ReportTime : IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly Stopwatch sw;
        private readonly string formattedString = "Used {0}";

        private bool isReportSql;

        public ReportTime()
        {
            sw = new Stopwatch();
            sw.Start();
        }

        public ReportTime(string formattedString)
        {
            this.formattedString = formattedString;
            sw = new Stopwatch();
            sw.Start();
        }

        public static ReportTime ReportSql(bool isReading, int serialNumber, string sql)
        {
            var formattedString = isReading ? "Read " : "Execute ";
            formattedString += "(" + serialNumber.ToString("D5") + ") Used {0}; " + sql;
            return new ReportTime(formattedString) { isReportSql = true };
        }

        public void Dispose()
        {
            sw.Stop();

            if (isReportSql)
            {
                Log.InfoFormat(formattedString, sw.Elapsed.TotalMilliseconds);
            }
            else
            {
                Log.DebugFormat(formattedString, sw.Elapsed.TotalMilliseconds);
#if DEBUG
                Console.WriteLine(formattedString, sw.Elapsed.TotalMilliseconds);
#endif
            }
        }
    }
}
