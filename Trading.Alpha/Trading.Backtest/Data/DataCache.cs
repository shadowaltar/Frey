using System;
using System.Collections.Generic;
using Trading.Common.Entities;

namespace Trading.Backtest.Data
{
    public class DataCache
    {
        private static readonly Lazy<DataCache> c = new Lazy<DataCache>(() => new DataCache());
        public static DataCache Instance { get { return c.Value; } }

        private readonly Dictionary<DateTime, Dictionary<long, Price>> priceCache = new Dictionary<DateTime, Dictionary<long, Price>>();
        private readonly Dictionary<long, Security> securityCache = new Dictionary<long, Security>();

        public static Dictionary<DateTime, Dictionary<long, Price>> PriceCache { get { return Instance.priceCache; } }
        public static Dictionary<long, Security> SecurityCache { get { return Instance.securityCache; } }
    }
}