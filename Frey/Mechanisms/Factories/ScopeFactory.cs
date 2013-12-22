using Automata.Core;
using Automata.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Automata.Mechanisms.Factories
{
    public class ScopeFactory
    {
        public static IDataScope DailyAllUnitedStatesStocks(int yearsAgo)
        {
            if (yearsAgo <= 0)
            {
                throw new ArgumentException();
            }

            var scope = new TestScope();
            var usa = Context.References.LookupCountry("US");
            var exchanges = Context.References.AllExchangesOf(usa).Values.ToList();
            var securities = Context.References.AllExchangeTradablesOf(exchanges).Values;
            scope.Countries.Add(usa);
            scope.Exchanges.AddRange(exchanges);
            scope.Securities.AddRange(securities);

            scope.End = DateTime.Now.Date;
            scope.Start = scope.End.AddYears(-yearsAgo);
            scope.PriceInterval = TimeSpan.FromDays(1);

            return scope;
        }
    }
}
