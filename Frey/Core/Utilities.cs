using System;
using Automata.Core.Extensions;

namespace Automata.Core
{
    public static class Utilities
    {
        private static readonly TimeZoneInfo LondonTimeZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
        private static readonly TimeZoneInfo AmericaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        private static readonly TimeZoneInfo JapanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");

        public static string Now
        {
            get { return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); }
        }

        public static string BracketNow
        {
            get { return "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "]"; }
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
            return "[" + time.ToString("yyyy-MM-dd HH:mm:ss") + "]";
        }

        public static string PrintPrecise(this double value)
        {
            return value.ToString("0.00000000");
        }

        public static string PrintForexPrecise(this double value)
        {
            return value.ToString("0.000000");
        }

        public static DateTime AmericaToUTC0(this DateTime time)
        {
            return TimeZoneInfo.ConvertTimeToUtc(time, AmericaTimeZone);
        }

        public static bool IsAmericaSummerTime(this DateTime time)
        {
            return AmericaTimeZone.IsDaylightSavingTime(time);
        }

        public static bool IsUnitedKingdomSummerTime(this DateTime time)
        {
            return LondonTimeZone.IsDaylightSavingTime(time);
        }

        /// <summary>
        /// Test if the given input time is within the trading session of forex market.
        /// </summary>
        /// <param name="time">Must be a UTC-0 time.</param>
        /// <returns></returns>
        public static bool IsForexMarketTradingSession(this DateTime time)
        {
            var asJapan = TimeZoneInfo.ConvertTimeFromUtc(time, JapanTimeZone);
            if (asJapan.DayOfWeek == DayOfWeek.Monday)
            {
                return asJapan.Hour > 8;
            }
            var asNewYork = TimeZoneInfo.ConvertTimeFromUtc(time, AmericaTimeZone);
            if (asNewYork.DayOfWeek == DayOfWeek.Friday)
            {
                return asNewYork.Hour < 17;
            }

            return time.IsBusinessDay();
        }
    }
}
