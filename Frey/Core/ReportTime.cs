using System;
using System.Diagnostics;

namespace Automata.Core
{

    public class ReportTime : IDisposable
    {
        private readonly Stopwatch sw;
        private readonly string formattedString = "Used {0}";

        public static ReportTime Start()
        {
            return new ReportTime();
        }

        public static ReportTime StartWithMessage(string formattedString)
        {
            return new ReportTime(formattedString);
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