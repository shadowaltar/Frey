﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Trading.Common
{
    public static class Constants
    {
        public const string IsoFormatDate = "yyyy-MM-dd";
        public const string IsoFormatDateTime = "yyyy-MM-dd HH:mm:ss";
        public const string IsoFormatTimestamp = "yyyy-MM-dd HH:mm:ss.ffffff";

        public static string UserName { get { return Environment.UserName.ToUpperInvariant(); } }
        public static DateTime MaxExpiryDate { get { return DateTime.MaxValue.Date; } }

        public static void InitializeDirectories()
        {
            var dirInfos = typeof(Constants).GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(p => p.Name.EndsWith("Directory"));
            foreach (var info in dirInfos)
            {
                var path = info.GetValue(typeof(Constants)).ToString();
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
        }

        public static string CurrentDirectory { get { return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); } }
        public static string CurrentDrive { get { return Path.GetPathRoot(Assembly.GetExecutingAssembly().Location); } }
        public static string PricesDirectory { get { return Path.Combine(CurrentDrive, @"Trading\DataFiles\Prices"); } }
        public static string SecurityListDirectory { get { return Path.Combine(CurrentDrive, @"Trading\DataFiles\SecurityList"); } }
        public static string MiscDirectory { get { return Path.Combine(CurrentDrive, @"Trading\DataFiles\OtherData"); } }
        public static string LogsDirectory { get { return Path.Combine(CurrentDrive, @"Trading\LogFiles"); } }
    }
}