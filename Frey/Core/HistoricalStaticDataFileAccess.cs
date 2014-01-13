using Automata.Core.Extensions;
using Automata.Entities;
using Automata.Mechanisms;
using System.Collections.Generic;
using System.Linq;

namespace Automata.Core
{
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
            Preprocessor = new DataProcessor(tradingScope.TargetPriceDuration);

            allHistoricalPrices.Clear();

            var prices = new HashSet<Price>();
            foreach (var security in securities)
            {
                var dynamicPrices = Context.ReadPricesFromDataFile(security, tradingScope.SourcePriceDuration, tradingScope.PriceSourceType);
                // Price lastPrice = null;
                var combinedPrices = Context.PreprocessPrices(dynamicPrices, tradingScope.TargetPriceDuration,
                    tradingScope.Start, tradingScope.End);
                prices.AddRange(combinedPrices);
            }

            var orderedPrices = prices.OrderBy(p => p.Time).ThenBy(p => p.Security.Code);
            foreach (var group in orderedPrices.GroupBy(p => p.Time).OrderBy(g => g.Key))
            {
                allHistoricalPrices.Enqueue(group.ToHashSet());
            }
            IsEnded = true;
        }
    }
}