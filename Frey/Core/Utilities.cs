using System;
using System.Threading;

namespace Automata.Core
{
    public static class Utilities
    {
        public static string Now
        {
            get { return DateTime.Now.ToString("yyyyMMdd HHmmss"); }
        }

        public static string BracketNow
        {
            get { return "[" + DateTime.Now.ToString("yyyyMMdd HHmmss") + "]"; }
        }

        public static void WriteTimedLine(string value)
        {
            Console.WriteLine(BracketNow + " " + value);
        }

        public static void WriteTimedLine(string value, params object[] arg)
        {
            Console.WriteLine(BracketNow + " " + value, arg);
        }

        public static string Print(this DateTime time)
        {
            return time.ToString("yyyyMMdd HHmmss");
        }

        public static string PrintBracket(this DateTime time)
        {
            return "[" + time.ToString("yyyyMMdd HHmmss") + "]";
        }
    }

    public static class IndexGenerator
    {
        private static readonly object lockSlim = new object();

        private static int id;

        public static int NextId()
        {
            lock (lockSlim)
            {
                Interlocked.Increment(ref id);
                return id;
            }
        }
    }
}
