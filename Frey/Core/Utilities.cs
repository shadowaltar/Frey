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

        public static string PrintBracket(this DateTime time)
        {
            return "[" + time.ToString("yyyyMMdd HH:mm:ss") + "]";
        }
    }
}
