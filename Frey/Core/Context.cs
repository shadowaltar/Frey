using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Automata.Core.Extensions;
using Automata.Entities;
using Automata.Mechanisms;

namespace Automata.Core
{
    public static class Context
    {
        static Context()
        {
            references = new TestReferences();
            references.Initialize();
        }

        public static string StaticDataFileDirectory { get { return "../../../../StaticDataFiles"; } }

        private static readonly ReferenceUniverse references;
        public static ReferenceUniverse References { get { return references; } }

        public static IEnumerable<Country> ReadCountriesFromDataFile()
        {
            var reader = new CsvFileReader();
            var filePath = Path.Combine(StaticDataFileDirectory, "Meta_Countries.csv");
            var data = reader.Read(filePath, true, '|');
            return data.Select(x => new Country { Id = x[0].ToInt(), Code = x[1], Name = x[2] });
        }

        public static IEnumerable<Exchange> ReadExchangesFromDataFile()
        {
            var reader = new CsvFileReader();
            var filePath = Path.Combine(StaticDataFileDirectory, "Meta_Exchanges.csv");
            var data = reader.Read(filePath, true, '|');
            return data.Select(x => new Exchange
            {
                Id = x[0].ToInt(),
                Code = x[1],
                Name = x[2],
                Mic = x[3],
                Country = References.LookupCountry(x[4])
            });
        }

        public static IEnumerable<Security> ReadSecuritiesFromDataFile()
        {
            var reader = new CsvFileReader();
            var filePath = Path.Combine(StaticDataFileDirectory, "Meta_ExchangeTradables.csv");
            var data = reader.Read(filePath, true, '|');

            foreach (var x in data)
            {
                if (x[2] == "Equity")
                {
                    var security = new Equity
                    {
                        Id = x[0].ToInt(),
                        Code = x[1],
                        Symbol = x[3],
                        Name = x[4],
                        Exchange = References.LookupExchange(x[5]),
                        Sector = x[6],
                        Description = x[7]
                    };
                    yield return security;
                }
                else if (x[2] == "ETF")
                {
                    var security = new ETF
                    {
                        Id = x[0].ToInt(),
                        Code = x[1],
                        Symbol = x[3],
                        Name = x[4],
                        Exchange = References.LookupExchange(x[5]),
                        Description = x[7]
                    };

                    yield return security;
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }

        public static IEnumerable<Price> ReadPricesFromDataFile(Security security)
        {
            var reader = new CsvFileReader();
            var fileName = security.Code.Replace(":", "_") + ".csv";
            var filePath = Path.Combine(StaticDataFileDirectory, fileName);
            var data = reader.Read(filePath, true);
            return Convert(DataPriceSourceType.YahooHistorical, data, security);
        }

        private static IEnumerable<Price> Convert(DataPriceSourceType priceSourceType, List<string[]> data, Security security)
        {
            if (priceSourceType == DataPriceSourceType.YahooHistorical)
            {
                var duration = TimeSpan.FromDays(1);
                return data.Select(x => new Price
                {
                    Time = x[0].ToDateTime("yyyy-MM-dd"),
                    Duration = duration,

                    Open = x[1].ToDouble(),
                    High = x[2].ToDouble(),
                    Low = x[3].ToDouble(),
                    Close = x[4].ToDouble(),
                    Volume = x[5].ToDouble(),
                    AdjustedClose = x[6].ToDouble(),

                    Security = security,
                });
            }
            throw new NotImplementedException();
        }
    }
}