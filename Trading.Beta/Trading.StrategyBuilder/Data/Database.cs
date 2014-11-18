using System;
using Trading.Common.Entities;
using Trading.Common.Utils;

namespace Trading.StrategyBuilder.Data
{
    public class Database
    {
        private static readonly Lazy<Database> factory = new Lazy<Database>(() => new Database());
        public static Database Instance { get { return factory.Value; } }

        private DualDictionary<DateTime, long, Price> prices = new DualDictionary<DateTime, long, Price>();
        public static DualDictionary<DateTime, long, Price> Prices { get { return Instance.prices; } }

        public static void CachePrices(DualDictionary<DateTime, long, Price> prices)
        {
            Instance.prices = prices;
        }
    }
}