using System;
using System.Collections.Generic;
using Trading.Common.Entities;

namespace Trading.Backtest.Data
{
    public class DataCache
    {
        private static Lazy<DataCache> c = new Lazy<DataCache>(() => new DataCache());
        public static DataCache Instance { get { return c.Value; } }

        private readonly Dictionary<DateTime, Dictionary<int, double>> volumeCache = new Dictionary<DateTime, Dictionary<int, double>>();
        private readonly Dictionary<int, Security> securityCache = new Dictionary<int, Security>();

        public static Dictionary<DateTime, Dictionary<int, double>> VolumeCache { get { return Instance.volumeCache; } }
        public static Dictionary<int, Security> SecurityCache { get { return Instance.securityCache; } }
    }
}