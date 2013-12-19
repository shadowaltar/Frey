using Automata.Entities;
using Automata.Mechanisms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Automata.Core
{
    public class DataAccess : IDisposable
    {
        public void Dispose()
        {

        }

        internal IEnumerable<Price> Read(IDataScope DataScope)
        {
            throw new NotImplementedException();
        }
    }

    public class HistoricalDatabaseAccess : DataAccess
    {
        public Action<HashSet<Price>> NewInfo;

        public HistoricalDatabaseAccess()
        {
            // init db conn
        }

        public HashSet<Price> Read()
        {
            // (fake) read db
            var securities = new Dictionary<int, Security>();
            var prices = new HashSet<Price>();

            // fake security 1
            var stock1 = SecurityUniverse.Lookup<Stock>(SecurityIdentifier.Code, "GOOG");
            securities.Add(stock1.Id, stock1);

            // fake price 1
            var price = new Price { Open = 100, High = 102, Low = 99, Close = 101 };
            var securityIdOfPrice = 1;
            price.Security = securities[securityIdOfPrice];

            prices.Add(price);

            return prices;
        }
    }
}
