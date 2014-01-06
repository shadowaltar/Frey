using Automata.Core;
using Automata.Core.Extensions;
using Automata.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Automata.Mechanisms.Factories
{
    public static class ScopeFactory
    {
        public static ITradingScope DailyAllUnitedStatesStocks(int yearsAgo)
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

            scope.End = DateTime.Parse("2013-12-13").Date;
            scope.Start = scope.End.AddYears(-yearsAgo);
            scope.PriceInterval = TimeSpan.FromDays(1);

            return scope;
        }

        public static ITradingScope DailyUsEtf(int yearsAgo)
        {
            if (yearsAgo <= 0)
            {
                throw new ArgumentException();
            }

            var scope = new TestScope();
            var usa = Context.References.LookupCountry("US");
            var exchanges = Context.References.AllExchangesOf(usa).Values.ToList();
            var securities = Context.References.AllExchangeTradablesOf(exchanges).Values.ToList();
            securities.RemoveAll(s => !(s is ETF));
            scope.Countries.Add(usa);
            scope.Exchanges.AddRange(exchanges);
            scope.Securities.AddRange(securities);

            scope.End = DateTime.Parse("2013-12-13").Date;
            scope.Start = scope.End.AddYears(-yearsAgo);
            scope.PriceInterval = TimeSpan.FromDays(1);

            return scope;
        }

        public static ITradingScope DailyUSEquity(int yearsAgo, string code)
        {
            if (yearsAgo <= 0)
            {
                throw new ArgumentException();
            }

            var scope = new TestScope();
            var usa = Context.References.LookupCountry("US");
            var exchanges = Context.References.AllExchangesOf(usa).Values.ToList();
            var securities = new List<Security> { Context.References.AllExchangeTradablesOf(exchanges)[code] };
            scope.Countries.Add(usa);
            scope.Exchanges.AddRange(exchanges);
            scope.Securities.AddRange(securities);

            scope.End = DateTime.Parse("2013-12-13").Date;
            scope.Start = scope.End.AddYears(-yearsAgo);
            scope.PriceInterval = TimeSpan.FromDays(1);

            return scope;
        }

        public static ITradingScope DailyPairStocks(int yearsAgo, string codeOne, string codeTwo)
        {
            if (yearsAgo <= 0)
            {
                throw new ArgumentException();
            }

            var scope = new TestScope();
            var s1 = Context.References.Lookup<Equity>(codeOne);
            var s2 = Context.References.Lookup<Equity>(codeTwo);
            scope.Securities.Add(s1);
            scope.Securities.Add(s2);
            scope.Countries.Add(s1.Exchange.Country);
            scope.Countries.AddIfNotExist(s2.Exchange.Country);
            scope.Exchanges.Add(s1.Exchange);
            scope.Exchanges.AddIfNotExist(s2.Exchange);

            scope.End = DateTime.Parse("2013-12-13").Date;
            scope.Start = scope.End.AddYears(-yearsAgo);
            scope.PriceInterval = TimeSpan.FromDays(1);

            return scope;
        }
    }
}
