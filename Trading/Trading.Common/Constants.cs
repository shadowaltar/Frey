using System;

namespace Trading.Common
{
    public static class Constants
    {
        public const string Imap = "IMAP";

        public static string UserName { get { return Environment.UserName.ToUpperInvariant(); } }
        public static DateTime MaxExpiryDate { get { return DateTime.MaxValue.Date; } }
    }
}