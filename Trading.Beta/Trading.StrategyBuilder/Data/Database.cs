using System;
using System.Collections.Generic;
using Trading.Common.Entities;
using Trading.Common.Utils;

namespace Trading.StrategyBuilder.Data
{
    public class Database
    {
        private static readonly Lazy<Database> factory = new Lazy<Database>(() => new Database());
        public static Database Instance { get { return factory.Value; } }

        private DualDictionary<int, long, Price> prices = new DualDictionary<int, long, Price>();
        public static DualDictionary<int, long, Price> Prices
        {
            get { return Instance.prices; }
            set { Instance.prices = value; }
        }

        private HashSet<int> holidays = new HashSet<int>();
        public static HashSet<int> Holidays
        {
            get { return Instance.holidays; }
            set { Instance.holidays = value; }
        }

        private Dictionary<long, Security> securities = new Dictionary<long, Security>();
        public static Dictionary<long, Security> Securities
        {
            get { return Instance.securities; }
            set { Instance.securities = value; }
        }
    }
}