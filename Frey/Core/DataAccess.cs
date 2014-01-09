using Automata.Core.Exceptions;
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

        public DataProcessor Preprocessor { get; set; }

        public override HashSet<Price> GetNextTimeslotPrices()
        {
            if (allHistoricalPrices.Count == 0)
                return null;

            return allHistoricalPrices.Dequeue();
        }

        public override void Initialize(ITradingScope tradingScope)
        {
            var securities = tradingScope.Securities;
            Preprocessor = new DataProcessor(tradingScope.TickDuration);

            allHistoricalPrices.Clear();

            var prices = new HashSet<Price>();
            foreach (var security in securities)
            {
                var dynamicPrices = Context.ReadPricesFromDataFile(security, tradingScope.TickDuration, tradingScope.PriceSourceType);
                Price lastPrice = null;
                foreach (var price in dynamicPrices)
                {
                    if (price.Time >= tradingScope.Start && price.Time <= tradingScope.End)
                    {
                        Preprocessor.CheckTimeGap(price, lastPrice);
                        prices.Add(price);
                        lastPrice = price;
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

    public class DataProcessor
    {
        private readonly TimeSpan tickDuration;
        private readonly TimeSpan doubleTickDuration;

        public DataProcessor(TimeSpan tickDuration)
        {
            this.tickDuration = tickDuration;
            doubleTickDuration = tickDuration.Multiply(2);
        }

        public List<Price> CheckTimeGap(Price current, Price last)
        {
            if (last == null)
                return null;

            var elapsed = current.Time - last.Time;

            if (elapsed == tickDuration)
            {
                return null;
            }

            if (elapsed < tickDuration)
            {
                throw new InvalidTickDurationException();
            }

            if (elapsed > tickDuration && elapsed < doubleTickDuration)
            {
                throw new InvalidTickDurationException();
            }

            var factor = elapsed.Divide(tickDuration);
            if (!factor.IsIntApprox())
                throw new InvalidTickDurationException();

            var results = new List<Price>();
            for (int i = 1; i <= factor.ToInt(); i++)
            {
                var p = new Price(current);
                p.Time = p.Time + tickDuration.Multiply(i);
                results.Add(p);
            }

            return results;
        }
    }
}
