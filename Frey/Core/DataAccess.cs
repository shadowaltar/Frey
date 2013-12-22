using System.IO;
using Automata.Core.Extensions;
using Automata.Entities;
using Automata.Mechanisms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Automata.Core
{
    public abstract class DataAccess : IDisposable
    {
        public void Dispose()
        {

        }

        public abstract HashSet<Price> Read();

        public abstract void Initialize(ITradingScope tradingScope);
    }

    public class HistoricalStaticDataFileAccess : DataAccess
    {
        private readonly PriceCache allHistoricalPrices = new PriceCache();
        private readonly List<DateTime> dataTimes = new List<DateTime>();
        private int nextDataTimeIndex;

        public override HashSet<Price> Read()
        {
            if (allHistoricalPrices.Count == 0 || nextDataTimeIndex >= dataTimes.Count)
                return null;

            var idx = nextDataTimeIndex;
            nextDataTimeIndex++;
            return allHistoricalPrices[dataTimes[idx]];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tradingScope"></param>
        public override void Initialize(ITradingScope tradingScope)
        {
            var securities = tradingScope.Securities;

            dataTimes.Clear();
            allHistoricalPrices.Clear();
            foreach (var security in securities)
            {
                foreach (var price in Context.ReadPricesFromDataFile(security))
                {
                    if (price.Time >= tradingScope.Start && price.Time <= tradingScope.End)
                    {
                        if (!allHistoricalPrices.ContainsKey(price.Time))
                        {
                            allHistoricalPrices[price.Time] = new HashSet<Price>();
                            dataTimes.Add(price.Time);
                        }
                        allHistoricalPrices[price.Time].Add(price);
                    }
                }
            }
            dataTimes.Reverse();
            dataTimes.Sort();
        }
    }
}
