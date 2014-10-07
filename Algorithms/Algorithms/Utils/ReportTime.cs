using System;
using System.Diagnostics;

namespace Algorithms.Utils
{
    public class ReportTime : IDisposable
    {
        private readonly Stopwatch sw;
        private readonly string formattedString = "Used {0}";

        public long Elapsed
        {
            get { return sw.ElapsedMilliseconds; }
        }

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

        public void Dispose()
        {
            sw.Stop();
            Console.WriteLine(formattedString, sw.Elapsed.TotalMilliseconds);
        }
    }
}