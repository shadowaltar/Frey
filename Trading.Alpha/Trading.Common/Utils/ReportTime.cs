using log4net;
using System;
using System.Diagnostics;

namespace Trading.Common.Utils
{
    public class ReportTime : IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly Stopwatch sw;
        private readonly string formattedString = "Used {0}";

        public string Message { get { return string.Format(formattedString, sw.Elapsed.TotalMilliseconds); } }

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

        public static ReportTime Start(string formattedString = "")
        {
            return formattedString.IsNullOrWhitespace() ? new ReportTime() : new ReportTime(formattedString);
        }

        public static ReportTime ReportSql(bool isReading, int serialNumber, string sql)
        {
            var formattedString = isReading ? "Read " : "Execute ";
            formattedString += "(" + serialNumber.ToString("D5") + ") Used {0}; " + sql;
            return new ReportTime(formattedString);
        }

        public void Dispose()
        {
            sw.Stop();
            Log.InfoFormat(Message);
            Console.WriteLine(Message);
        }
    }
}
