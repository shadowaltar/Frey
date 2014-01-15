using Automata.Core;
using Automata.Core.Exceptions;
using Automata.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Automata.Mechanisms
{
    public abstract class ReferenceUniverse : IReferenceUniverse
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

        public T Lookup<T>(string code) where T : Security
        {
            try
            {
                SecurityCacheLock.EnterReadLock();

                if (Securities.ContainsKey(code))
                    return Securities[code] as T;
                throw new SecurityReferenceException();
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

                if (Countries.ContainsKey(code))
                    return Countries[code];
                throw new StaticDataReferenceException();
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

                if (Exchanges.ContainsKey(code))
                    return Exchanges[code];
                throw new StaticDataReferenceException();
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

        protected abstract void InitializeExchanges();

        protected abstract void InitializeCountries();

        protected abstract void InitializeSecurities();
    }

    public interface IReferenceUniverse
    {
        void Initialize();

        T Lookup<T>(string code) where T : Security;
        Country LookupCountry(string code);
        Exchange LookupExchange(string code);
        Dictionary<string, Exchange> AllExchangesOf(params Country[] countries);
        Dictionary<string, Security> AllExchangeTradablesOf(params Exchange[] exchanges);
        Dictionary<string, Security> AllExchangeTradablesOf(IEnumerable<Exchange> exchanges);
    }

    public class TestReferences : ReferenceUniverse
    {
        protected override void InitializeCountries()
        {
            try
            {
                CountryCacheLock.EnterWriteLock();
                Countries.Clear();
                foreach (var country in StaticFileDataAccess.ReadCountriesFromDataFile())
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
                foreach (var exchange in StaticFileDataAccess.ReadExchangesFromDataFile())
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
                foreach (var security in StaticFileDataAccess.ReadSecuritiesFromDataFile())
                {
                    Securities[security.Code] = security;
                }
                foreach (var security in StaticFileDataAccess.ReadCurrenciesFromDataFile())
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
