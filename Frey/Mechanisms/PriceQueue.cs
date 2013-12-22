using System.Collections.Concurrent;
using System.Collections.Generic;
using Automata.Core.Extensions;
using Automata.Entities;

namespace Automata.Mechanisms
{
    public class PriceQueue : ConcurrentQueue<HashSet<Price>>
    {
        private PriceQueue() { }

        public static PriceQueue New()
        {
            return new PriceQueue();
        }

        public HashSet<Price> NextItems()
        {
            try
            {
                HashSet<Price> result;
                return TryDequeue(out result) ? result : null;
            }
            catch
            {
                return null;
            }
        }

        public void AddItems(IEnumerable<Price> prices)
        {
            Enqueue(prices.ToHashSet());
        }
    }
}
