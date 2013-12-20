using Automata.Core.Exceptions;

using Automata.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;

namespace Automata.Mechanisms
{
    public static class SecurityUniverse
    {
        private static readonly ReaderWriterLockSlim securityCacheLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        private static readonly ReaderWriterLockSlim exchangeCacheLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        private static readonly ReaderWriterLockSlim countryCacheLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        private static readonly Dictionary<string, Security> securities = new Dictionary<string, Security>();
        private static readonly Dictionary<string, Country> countries = new Dictionary<string, Country>();
        private static readonly Dictionary<string, Exchange> exchanges = new Dictionary<string, Exchange>();

        static SecurityUniverse()
        {
            InitializeCountries();
            InitializeExchanges();
            InitializeSecurities();
        }

        public static T Lookup<T>(SecurityIdentifier securityIdentifier, string value)
            where T : Security
        {
            try
            {
                securityCacheLock.EnterReadLock();

                var result = LookupInternal<T>(securityIdentifier, value);
                if (result == null)
                    throw new SecurityReferenceException();
                return result;
            }
            finally
            {
                securityCacheLock.ExitReadLock();
            }
        }

        public static bool TryLookup<T>(SecurityIdentifier securityIdentifier, string value, out Security result)
            where T : Security
        {
            try
            {
                securityCacheLock.EnterReadLock();

                result = null;
                var r = LookupInternal<T>(securityIdentifier, value);
                if (r == null)
                    return false;
                result = r;
                return true;
            }
            finally
            {
                securityCacheLock.ExitReadLock();
            }
        }

        public static Country LookupCountry(string code)
        {
            try
            {
                countryCacheLock.EnterReadLock();

                var result = LookupCountryByCodeInternal(code);
                if (result == null)
                    throw new StaticDataReferenceException();
                return result;
            }
            finally
            {
                countryCacheLock.ExitReadLock();
            }
        }

        public static Exchange LookupExchange(string code)
        {
            try
            {
                exchangeCacheLock.EnterReadLock();

                var result = LookupExchangeByCodeInternal(code);
                if (result == null)
                    throw new StaticDataReferenceException();
                return result;
            }
            finally
            {
                exchangeCacheLock.ExitReadLock();
            }
        }

        public static Dictionary<string, Exchange> AllExchangesOf(params Country[] countries)
        {
            try
            {
                exchangeCacheLock.EnterReadLock();

                return exchanges.Where(e => countries.Contains(e.Value.Country)).ToDictionary(e => e.Key, e => e.Value);
            }
            finally
            {
                exchangeCacheLock.ExitReadLock();
            }
        }

        public static Dictionary<string, Security> AllExchangeTradablesOf(params Exchange[] exchanges)
        {
            return AllExchangeTradablesOf((IEnumerable<Exchange>)exchanges);
        }

        public static Dictionary<string, Security> AllExchangeTradablesOf(IEnumerable<Exchange> exchanges)
        {
            try
            {
                securityCacheLock.EnterReadLock();

                var et = securities.Where(s => { var v = s.Value is ExchangeTradable; return v; });
                var re = et.Where(s => exchanges.Contains(((ExchangeTradable)s.Value).Exchange));
                var result = re.ToDictionary(e => e.Key, e => e.Value);
                return result;
            }
            finally
            {
                securityCacheLock.ExitReadLock();
            }
        }

        private static void InitializeExchanges()
        {
            try
            {
                exchangeCacheLock.EnterWriteLock();
                exchanges["NASDAQ"] = new Exchange { Id = 1, Code = "NASDAQ", Name = "NASDAQ", Mic = "XNAS", Country = LookupCountry("US") };
                exchanges["NYSE"] = new Exchange { Id = 2, Code = "NYSE", Name = "NEW YORK STOCK EXCHANGE", Mic = "XNYS", Country = LookupCountry("US") };
                exchanges["HKEX"] = new Exchange { Id = 3, Code = "HKEX", Name = "HONG KONG EXCHANGES AND CLEARING", Mic = "XHKG", Country = LookupCountry("HK") };
                exchanges["TSE"] = new Exchange { Id = 4, Code = "TSE", Name = "TOKYO STOCK EXCHANGE", Mic = "XJPX", Country = LookupCountry("JP") };
            }
            finally
            {
                exchangeCacheLock.ExitWriteLock();
            }
        }

        private static void InitializeCountries()
        {
            try
            {
                countryCacheLock.EnterWriteLock();
                countries["US"] = new Country { Id = 1, Code = "US", Name = "United States" };
                countries["HK"] = new Country { Id = 2, Code = "HK", Name = "Hong Kong" };
                countries["CN"] = new Country { Id = 3, Code = "CN", Name = "China" };
                countries["JP"] = new Country { Id = 4, Code = "JP", Name = "Japan" };
            }
            finally
            {
                countryCacheLock.ExitWriteLock();
            }
        }

        private static void InitializeSecurities()
        {
            try
            {
                securityCacheLock.EnterWriteLock();
                securities["AAPL"] = new Stock { Id = 1, Code = "AAPL", Name = "Apple Inc.", Exchange = LookupExchange("NASDAQ") };
                securities["GOOG"] = new Stock { Id = 2, Code = "GOOG", Name = "Google Inc.", Exchange = LookupExchange("NASDAQ") };
                securities["SPY"] = new ExchangeTradedFund { Id = 3, Code = "SPY", Name = "SPDR S&P 500", Exchange = LookupExchange("NYSE") };
                securities["EEM"] = new ExchangeTradedFund { Id = 4, Code = "EEM", Name = "iShares MSCI Emerging Markets", Exchange = LookupExchange("NYSE") };
            }
            finally
            {
                securityCacheLock.ExitWriteLock();
            }
        }

        private static T LookupInternal<T>(SecurityIdentifier securityIdentifier, string value) where T : Security
        {
            switch (value)
            {
                case "AAPL":
                    if (typeof(T) == typeof(Stock))
                        return new Stock { Id = 1, Code = "AAPL", Name = "Apple Inc." } as T;
                    break;
                case "GOOG":
                    if (typeof(T) == typeof(Stock))
                        return new Stock { Id = 2, Code = "GOOG", Name = "Google Inc." } as T;
                    break;
            }
            return null;
        }

        private static Country LookupCountryByCodeInternal(string code)
        {
            try
            {
                if (countries.ContainsKey(code))
                    return countries[code];
            }
            catch
            {
                return null;
            }
            return null;
        }

        private static Exchange LookupExchangeByCodeInternal(string code)
        {
            try
            {
                if (exchanges.ContainsKey(code))
                    return exchanges[code];
            }
            catch
            {
                return null;
            }
            return null;
        }
    }
}
