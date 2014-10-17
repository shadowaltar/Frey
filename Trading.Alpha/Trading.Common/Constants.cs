using System;

namespace Trading.Common
{
    public static class Constants
    {
        public const string IsoFormatDate = "yyyy-MM-dd";
        public const string IsoFormatDateTime = "yyyy-MM-dd HH:mm:ss";
        public const string IsoFormatTimestamp = "yyyy-MM-dd HH:mm:ss.ffffff";

        public static string UserName { get { return Environment.UserName.ToUpperInvariant(); } }
        public static DateTime MaxExpiryDate { get { return DateTime.MaxValue.Date; } }
    }
}