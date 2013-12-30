using Automata.Core.Extensions;
using Automata.Entities;
using Automata.Mechanisms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Automata.Core
{
    public abstract class DataAccess : IDisposable
    {
        public virtual void Dispose()
        {
        }

        public abstract HashSet<Price> GetNextTimeslotPrices();

        public abstract void Initialize(ITradingScope tradingScope);
    }

    public class HistoricalStaticDataFileAccess : DataAccess
    {
        private readonly Queue<HashSet<Price>> allHistoricalPrices = new Queue<HashSet<Price>>();

        public override HashSet<Price> GetNextTimeslotPrices()
        {
            if (allHistoricalPrices.Count == 0)
                return null;

            return allHistoricalPrices.Dequeue();
        }

        public override void Initialize(ITradingScope tradingScope)
        {
            var securities = tradingScope.Securities;

            allHistoricalPrices.Clear();
            var prices = new HashSet<Price>();
            foreach (var security in securities)
            {
                foreach (var price in Context.ReadPricesFromDataFile(security))
                {
                    if (price.Time >= tradingScope.Start && price.Time <= tradingScope.End)
                    {
                        prices.Add(price);
                    }
                }
            }

            var orderedPrices = prices.OrderBy(p => p.Time).ThenBy(p => p.Security.Code);
            foreach (var group in orderedPrices.GroupBy(p => p.Time).OrderBy(g => g.Key))
            {
                allHistoricalPrices.Enqueue(group.ToHashSet());
            }
        }
    }
}
