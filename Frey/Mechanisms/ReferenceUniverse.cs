using Automata.Core;
using Automata.Core.Exceptions;
using Automata.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Automata.Mechanisms
{
    public class ReferenceUniverse
    {
        protected readonly ReaderWriterLockSlim CountryCacheLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        protected readonly ReaderWriterLockSlim ExchangeCacheLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        protected readonly ReaderWriterLockSlim SecurityCacheLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        protected readonly Dictionary<string, Country> Countries = new Dictionary<string, Country>();
        protected readonly Dictionary<string, Exchange> Exchanges = new Dictionary<string, Exchange>();
        protected readonly Dictionary<string, Security> Securities = new Dictionary<string, Security>();

        public void Initialize()
        {
            InitializeCountries();
            InitializeExchanges();
            InitializeSecurities();
        }

        public T Lookup<T>(SecurityIdentifier securityIdentifier, string value)
            where T : Security
        {
            try
            {
                SecurityCacheLock.EnterReadLock();

                var result = LookupInternal<T>(securityIdentifier, value);
                if (result == null)
                    throw new SecurityReferenceException();
                return result;
            }
            finally
            {
                SecurityCacheLock.ExitReadLock();
            }
        }

        public bool TryLookup<T>(SecurityIdentifier securityIdentifier, string value, out Security result)
            where T : Security
        {
            try
            {
                SecurityCacheLock.EnterReadLock();

                result = null;
                var r = LookupInternal<T>(securityIdentifier, value);
                if (r == null)
                    return false;
                result = r;
                return true;
            }
            finally
            {
                SecurityCacheLock.ExitReadLock();
            }
        }

        public Country LookupCountry(string code)
        {
            try
            {
                CountryCacheLock.EnterReadLock();

                var result = LookupCountryByCodeInternal(code);
                if (result == null)
                    throw new StaticDataReferenceException();
                return result;
            }
            finally
            {
                CountryCacheLock.ExitReadLock();
            }
        }

        public Exchange LookupExchange(string code)
        {
            try
            {
                ExchangeCacheLock.EnterReadLock();

                var result = LookupExchangeByCodeInternal(code);
                if (result == null)
                    throw new StaticDataReferenceException();
                return result;
            }
            finally
            {
                ExchangeCacheLock.ExitReadLock();
            }
        }

        public Dictionary<string, Exchange> AllExchangesOf(params Country[] countries)
        {
            try
            {
                ExchangeCacheLock.EnterReadLock();

                return Exchanges.Where(e => countries.Contains(e.Value.Country)).ToDictionary(e => e.Key, e => e.Value);
            }
            finally
            {
                ExchangeCacheLock.ExitReadLock();
            }
        }

        public Dictionary<string, Security> AllExchangeTradablesOf(params Exchange[] exchanges)
        {
            return AllExchangeTradablesOf((IEnumerable<Exchange>)exchanges);
        }

        public Dictionary<string, Security> AllExchangeTradablesOf(IEnumerable<Exchange> exchanges)
        {
            try
            {
                SecurityCacheLock.EnterReadLock();

                var et = Securities.Where(s => { var v = s.Value is ExchangeTradable; return v; });
                var re = et.Where(s => exchanges.Contains(((ExchangeTradable)s.Value).Exchange));
                var result = re.ToDictionary(e => e.Key, e => e.Value);
                return result;
            }
            finally
            {
                SecurityCacheLock.ExitReadLock();
            }
        }

        protected virtual void InitializeExchanges()
        {
            try
            {
                ExchangeCacheLock.EnterWriteLock();
                Exchanges["NASDAQ"] = new Exchange { Id = 1, Code = "NASDAQ", Name = "NASDAQ", Mic = "XNAS", Country = LookupCountry("US") };
                Exchanges["NYSE"] = new Exchange { Id = 2, Code = "NYSE", Name = "NEW YORK STOCK EXCHANGE", Mic = "XNYS", Country = LookupCountry("US") };
                Exchanges["HKEX"] = new Exchange { Id = 3, Code = "HKEX", Name = "HONG KONG EXCHANGES AND CLEARING", Mic = "XHKG", Country = LookupCountry("HK") };
                Exchanges["TSE"] = new Exchange { Id = 4, Code = "TSE", Name = "TOKYO STOCK EXCHANGE", Mic = "XJPX", Country = LookupCountry("JP") };
            }
            finally
            {
                ExchangeCacheLock.ExitWriteLock();
            }
        }

        protected virtual void InitializeCountries()
        {
            try
            {
                CountryCacheLock.EnterWriteLock();
                Countries["US"] = new Country { Id = 1, Code = "US", Name = "United States" };
                Countries["HK"] = new Country { Id = 2, Code = "HK", Name = "Hong Kong" };
                Countries["CN"] = new Country { Id = 3, Code = "CN", Name = "China" };
                Countries["JP"] = new Country { Id = 4, Code = "JP", Name = "Japan" };
            }
            finally
            {
                CountryCacheLock.ExitWriteLock();
            }
        }

        protected virtual void InitializeSecurities()
        {
            try
            {
                SecurityCacheLock.EnterWriteLock();
                Securities["AAPL"] = new Equity { Id = 1, Code = "AAPL", Name = "Apple Inc.", Exchange = LookupExchange("NASDAQ") };
                Securities["GOOG"] = new Equity { Id = 2, Code = "GOOG", Name = "Google Inc.", Exchange = LookupExchange("NASDAQ") };
                Securities["SPY"] = new ETF { Id = 3, Code = "SPY", Name = "SPDR S&P 500", Exchange = LookupExchange("NYSE") };
                Securities["EEM"] = new ETF { Id = 4, Code = "EEM", Name = "iShares MSCI Emerging Markets", Exchange = LookupExchange("NYSE") };
            }
            finally
            {
                SecurityCacheLock.ExitWriteLock();
            }
        }

        private T LookupInternal<T>(SecurityIdentifier securityIdentifier, string value) where T : Security
        {
            switch (value)
            {
                case "AAPL":
                    if (typeof(T) == typeof(Equity))
                        return new Equity { Id = 1, Code = "AAPL", Name = "Apple Inc." } as T;
                    break;
                case "GOOG":
                    if (typeof(T) == typeof(Equity))
                        return new Equity { Id = 2, Code = "GOOG", Name = "Google Inc." } as T;
                    break;
            }
            return null;
        }

        private Country LookupCountryByCodeInternal(string code)
        {
            try
            {
                if (Countries.ContainsKey(code))
                    return Countries[code];
            }
            catch
            {
                return null;
            }
            return null;
        }

        private Exchange LookupExchangeByCodeInternal(string code)
        {
            try
            {
                if (Exchanges.ContainsKey(code))
                    return Exchanges[code];
            }
            catch
            {
                return null;
            }
            return null;
        }
    }

    public class TestReferences : ReferenceUniverse
    {
        protected override void InitializeCountries()
        {
            try
            {
                CountryCacheLock.EnterWriteLock();
                Countries.Clear();
                foreach (var country in Context.ReadCountriesFromDataFile())
                {
                    Countries[country.Code] = country;
                }
            }
            finally
            {
                CountryCacheLock.ExitWriteLock();
            }
        }

        protected override void InitializeExchanges()
        {
            try
            {
                ExchangeCacheLock.EnterWriteLock();
                Exchanges.Clear();
                foreach (var exchange in Context.ReadExchangesFromDataFile())
                {
                    Exchanges[exchange.Code] = exchange;
                }
            }
            finally
            {
                ExchangeCacheLock.ExitWriteLock();
            }
        }

        protected override void InitializeSecurities()
        {
            try
            {
                SecurityCacheLock.EnterWriteLock();
                Securities.Clear();
                foreach (var security in Context.ReadSecuritiesFromDataFile())
                {
                    Securities[security.Code] = security;
                }
            }
            finally
            {
                SecurityCacheLock.ExitWriteLock();
            }
        }
    }
}
