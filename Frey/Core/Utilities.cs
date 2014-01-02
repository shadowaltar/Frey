using System;

namespace Automata.Core
{
    public static class Utilities
    {
        public static string Now
        {
            get { return DateTime.Now.ToString("yyyyMMdd HH:mm:ss"); }
        }

        public static string BracketNow
        {
            get { return "[" + DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "]"; }
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
            return time.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static string PrintBracket(this DateTime time)
        {
            return "[" + time.ToString("yyyyMMdd HH:mm:ss") + "]";
        }
    }
}
