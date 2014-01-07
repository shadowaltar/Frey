using Automata.Core.Extensions;
using Automata.Entities;
using Automata.Mechanisms;
using Ninject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Automata.Core
{
    public static class Context
    {
        public static void Initialize(Objects objects)
        {
            References = objects.Get<IReferenceUniverse>();
            References.Initialize();
        }

        public static string StaticDataFileDirectory { get { return "../../../../StaticDataFiles"; } }

        public static IReferenceUniverse References { get; private set; }
        public const double DoubleTolerance = 1E-8;

        public static void DownloadYahooPriceFiles()
        {
            var reader = new CsvFileReader();
            var rows = reader.Read(Path.Combine(StaticDataFileDirectory, "Meta_ExchangeTradables.csv"), true, '|');
            foreach (var code in rows.Select(r => r[1]))
            {
                var exchange = code.Split(':')[0];
                var symbol = code.Split(':')[1];
                YahooStockPriceDownloader.Download(symbol, exchange, StaticDataFileDirectory);
            }
        }

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

        public static IEnumerable<Security> ReadCurrenciesFromDataFile()
        {
            var reader = new CsvFileReader();
            var filePath = Path.Combine(StaticDataFileDirectory, "Meta_Currency.csv");
            var data = reader.Read(filePath, true, '|');
            foreach (var x in data)
            {
                switch (x[1])
                {
                    case "FOREX":
                        var forex = new Forex
                        {
                            Id = x[0].ToInt(),
                            Code = x[2],
                            Description = x[3],
                            Base = (Currency)Enum.Parse(typeof(Currency), x[2].Substring(3)),
                            Quote = (Currency)Enum.Parse(typeof(Currency), x[2].Substring(0, 3)),
                        };
                        yield return forex;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }
        public static IEnumerable<Security> ReadSecuritiesFromDataFile()
        {
            var reader = new CsvFileReader();
            var filePath = Path.Combine(StaticDataFileDirectory, "Meta_ExchangeTradables.csv");
            var data = reader.Read(filePath, true, '|');

            foreach (var x in data)
            {
                switch (x[2])
                {
                    case "Equity":
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
                        break;
                    case "ETF":
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
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public static IEnumerable<Price> ReadPricesFromDataFile(Security security, TimeSpan duration, DataPriceSourceType sourceType = DataPriceSourceType.Unknown)
        {
            var reader = new CsvFileReader();
            var fileName = security.Code.Replace(":", "_") + ".csv";
            var filePath = Path.Combine(StaticDataFileDirectory, fileName);
            var data = reader.Read(filePath, true);
            return Convert(data, security, duration, sourceType);
        }

        //public static IEnumerable<Price> ReadForexPriceFromDataFile(string symbol)
        //{

        //}

        private static IEnumerable<Price> Convert(IEnumerable<string[]> data, Security security, TimeSpan duration, DataPriceSourceType priceSourceType = DataPriceSourceType.Unknown)
        {
            switch (priceSourceType)
            {
                case DataPriceSourceType.YahooHistorical:
                    foreach (var datum in data)
                    {
                        yield return new Price
                        {
                            Time = datum[0].ToDateTime("yyyy-MM-dd"),
                            Duration = duration,

                            Open = datum[1].ToDouble(),
                            High = datum[2].ToDouble(),
                            Low = datum[3].ToDouble(),
                            Close = datum[4].ToDouble(),
                            Volume = datum[5].ToDouble(),
                            AdjustedClose = datum[6].ToDouble(),

                            Security = security,
                        };
                    }
                    break;
                case DataPriceSourceType.DailyFXHistorical:
                    foreach (var x in data)
                    {
                        yield return new Price
                        {
                            Time = (x[0] + x[1]).ToDateTime("M/d/yyyyHH:mm:ss"),
                            Duration = duration,

                            Open = x[2].ToDouble(),
                            High = x[3].ToDouble(),
                            Low = x[4].ToDouble(),
                            Close = x[5].ToDouble(),

                            Security = security,
                        };
                    }
                    break;
            }
        }
    }
}