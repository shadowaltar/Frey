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

        public abstract HashSet<Price> Read(IDataScope dataScope);

        public abstract void Initialize();
    }

    public class HistoricalDatabaseAccess : DataAccess
    {
        private List<HashSet<Price>> allHistoricalPrices = new List<HashSet<Price>>();

        public Action<HashSet<Price>> NewInfo;

        public override HashSet<Price> Read(IDataScope dataScope)
        {
            // (fake) read db
            var securities = dataScope.Securities;
            var prices = new HashSet<Price>();

            // fake price 1
            var price = new Price { Open = 100, High = 102, Low = 99, Close = 101 };
            var securityIdOfPrice = 1;
            price.Security = securities[securityIdOfPrice];

            prices.Add(price);

            return prices;
        }

        public override void Initialize()
        {
            var reader = new CsvFileReader();
            var data = reader.Read("../TestData/NYSE_SPY.csv", true);
            // the data read here is grouped by a security.
            // need to regroup it by datetime.
        }
    }
}
