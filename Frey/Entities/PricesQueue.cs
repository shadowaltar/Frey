using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Automata.Core.Utils;

namespace Automata.Entities
{
    public class PricesQueue : ConcurrentQueue<HashSet<Price>>
    {
        private PricesQueue() { }

        public static PricesQueue New()
        {
            return new PricesQueue();
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
