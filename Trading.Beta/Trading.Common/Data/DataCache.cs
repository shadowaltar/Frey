using System;
using System.Collections.Generic;
using System.Security.Policy;
using Trading.Common.Entities;

namespace Trading.Common.Data
{
    public class DataCache
    {
        private static readonly object syncRoot = new object();
        private static readonly Lazy<DataCache> c = new Lazy<DataCache>(() => new DataCache());
        public static DataCache Instance { get { lock(syncRoot) return c.Value; } }

        private readonly Dictionary<DateTime, Dictionary<long, Price>> priceCache = new Dictionary<DateTime, Dictionary<long, Price>>();
        private readonly Dictionary<long, Security> securityCache = new Dictionary<long, Security>();
        private readonly Dictionary<string, long> securityCodeMap = new Dictionary<string, long>();
        private readonly HashSet<int> holidayCache = new HashSet<int>();

        public static Dictionary<DateTime, Dictionary<long, Price>> PriceCache { get { return Instance.priceCache; } }
        public static Dictionary<long, Security> SecurityCache { get { return Instance.securityCache; } }
        public static HashSet<int> HolidayCache { get { return Instance.holidayCache; } }
        public static Dictionary<string, long> SecurityCodeMap { get { return Instance.securityCodeMap; } }

        public static string CommonBenchmarkCode { get { return "^GSPC"; } }
    }
}