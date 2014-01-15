using System;
using System.IO;
using Automata.Core.Extensions;
using Automata.Entities;
using Automata.Mechanisms;
using System.Collections.Generic;
using System.Linq;

namespace Automata.Core
{
    public class StaticFileDataAccess : DataAccess
    {
        private readonly Queue<HashSet<Price>> allHistoricalPrices = new Queue<HashSet<Price>>();

        public DataProcessor Preprocessor { get; set; }

        public override HashSet<Price> GetNextTimeslotPrices()
        {
            if (allHistoricalPrices.Count == 0)
                return null;

            return allHistoricalPrices.Dequeue();
        }

        public override void Initialize(ITradingScope tradingScope)
        {
            var securities = tradingScope.Securities;
            Preprocessor = new DataProcessor(tradingScope.TargetPriceDuration);

            allHistoricalPrices.Clear();

            var prices = new HashSet<Price>();
            foreach (var security in securities)
            {
                var dynamicPrices = ReadPricesFromDataFile(security, tradingScope.SourcePriceDuration,
                    tradingScope.PriceSourceType, tradingScope.PriceTimeZoneType);
                var combinedPrices = PreprocessStaticPrices(dynamicPrices, tradingScope.TargetPriceDuration,
                    tradingScope.Start, tradingScope.End);
                prices.AddRange(combinedPrices);
            }

            var orderedPrices = prices.OrderBy(p => p.Time).ThenBy(p => p.Security.Code);
            foreach (var group in orderedPrices.GroupBy(p => p.Time).OrderBy(g => g.Key))
            {
                allHistoricalPrices.Enqueue(group.ToHashSet());
            }
            IsEnded = true;
        }

        public static void DownloadYahooPriceFiles()
        {
            var rows = CsvFileAccess.Read(Path.Combine(Context.StaticDataFileDirectory,
                "Meta_ExchangeTradables.csv"), true, '|');
            foreach (var code in rows.Select(r => r[1]))
            {
                var exchange = code.Split(':')[0];
                var symbol = code.Split(':')[1];
                YahooStockPriceDownloader.Download(symbol, exchange, Context.StaticDataFileDirectory);
            }
        }

        public static IEnumerable<Country> ReadCountriesFromDataFile()
        {
            var filePath = Path.Combine(Context.StaticDataFileDirectory, "Meta_Countries.csv");
            var data = CsvFileAccess.Read(filePath, true, '|');
            return data.Select(x => new Country { Id = x[0].ToInt(), Code = x[1], Name = x[2] });
        }

        public static IEnumerable<Exchange> ReadExchangesFromDataFile()
        {
            var filePath = Path.Combine(Context.StaticDataFileDirectory, "Meta_Exchanges.csv");
            var data = CsvFileAccess.Read(filePath, true, '|');
            return data.Select(x => new Exchange
            {
                Id = x[0].ToInt(),
                Code = x[1],
                Name = x[2],
                Mic = x[3],
                Country = Context.References.LookupCountry(x[4])
            });
        }

        public static IEnumerable<Security> ReadCurrenciesFromDataFile()
        {
            var filePath = Path.Combine(Context.StaticDataFileDirectory, "Meta_Currency.csv");
            var data = CsvFileAccess.Read(filePath, true, '|');
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
            var filePath = Path.Combine(Context.StaticDataFileDirectory, "Meta_ExchangeTradables.csv");
            var data = CsvFileAccess.Read(filePath, true, '|');

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
                                Exchange = Context.References.LookupExchange(x[5]),
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
                                Exchange = Context.References.LookupExchange(x[5]),
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

        public static IEnumerable<Price> ReadPricesFromDataFile(Security security, TimeSpan duration,
            PriceSourceType sourceType = PriceSourceType.Unknown,
            TimeZoneType priceTimeZoneType = TimeZoneType.UTC)
        {
            var fileName = security.Code.Replace(":", "_") + ".csv";
            var filePath = Path.Combine(Context.StaticDataFileDirectory, fileName);
            var data = CsvFileAccess.Read(filePath, true);
            return Context.Convert(data, security, duration, sourceType, priceTimeZoneType);
        }

        /// <summary>
        /// Preprocess the raw prices. Here we can always assume the prices are of the same asset class
        /// because they come from the same file of prices.
        /// </summary>
        /// <param name="prices"></param>
        /// <param name="newDuration"></param>
        /// <param name="scopeStart"></param>
        /// <param name="scopeEnd"></param>
        /// <returns></returns>
        public static IEnumerable<Price> PreprocessStaticPrices(IEnumerable<Price> prices, TimeSpan newDuration,
            DateTime scopeStart, DateTime scopeEnd)
        {
            var cache = new List<Price>();
            Price firstPrice = null;
            TimeRange sessionRange = null;
            TimeRange nextSessionRange = null;
            TimeRange tickTimeRange = null;
            TimeRange nextTickTimeRange = null;

            var shouldCombine = false;

            foreach (var price in prices)
            {
                if (price.Time > scopeEnd)
                    break;
                if (price.Time < scopeStart)
                    continue;

                if (firstPrice == null)
                {
                    firstPrice = price;
                    sessionRange = FindTradingSession(price.Time, price.Security);
                    nextSessionRange = FindTradingSession(price.Time.AddWeeks(1), price.Security);
                }

                // if between two trading sessions (nontrading hours), skip the price
                if (!sessionRange.Contains(price.Time) && !nextSessionRange.Contains(price.Time))
                    continue;

                // if already in next trading session, shift the sessions to the next
                if (nextSessionRange.Contains(price.Time))
                {
                    sessionRange = nextSessionRange;
                    nextSessionRange = FindTradingSession(sessionRange.Start.AddWeeks(1));
                }

                // todo, refactor the '=' condition first.
                if (price.Duration >= newDuration)
                {
                    yield return price;
                }
                else
                {
                    if (tickTimeRange == null)
                    {
                        tickTimeRange = FindTickTimeRange(price.Time, sessionRange.Start, newDuration);
                        nextTickTimeRange = tickTimeRange.Add(newDuration);
                    }

                    // if already in next tick duration, shift the durations to the next
                    // and then we should combine the gathered ticks.
                    if (nextTickTimeRange.Contains(price.Time))
                    {
                        shouldCombine = true;
                    }

                    // gather the prices into current timeslot.
                    if (!shouldCombine)
                    {
                        cache.Add(price);
                        continue;
                    }

                    // combine then return the result
                    yield return Price.Combine(cache, tickTimeRange.Start, newDuration);

                    // clean up
                    cache.Clear();
                    shouldCombine = false;

                    tickTimeRange = nextTickTimeRange;
                    // already guaranteed the price time will fall into the sessionRange in above logic.
                    nextTickTimeRange = GetNextTickTimeRange(tickTimeRange, newDuration,
                        sessionRange, nextSessionRange);

                    cache.Add(price);
                }
            }
        }

        public static TimeRange FindTradingSession(DateTime anyDateTime, Security security)
        {
            if (security is Forex)
            {
                int offset = anyDateTime.DayOfWeek - DayOfWeek.Monday;

                var nearestLastMonday = anyDateTime.AddDays(-offset);
                if (nearestLastMonday > anyDateTime) // when 'time' is closer to the weekend, need find one week earlier date
                {
                    nearestLastMonday = nearestLastMonday.AddDays(-7);
                }
                return FindTradingSession(nearestLastMonday.Date);
            }
            if (security is ExchangeTradable)
            {
            }
            throw new NotImplementedException();
        }

        public static TimeRange FindTradingSession(DateTime sessionStart)
        {
            var start = sessionStart;
            // UTC+0 9pm is EST (GMT-5) 4pm.
            var end = new DateTime(sessionStart.Ticks).AddDays(4).AddHours(17).AmericaToUTC0();
            return new TimeRange(start, end);
        }

        public static TimeRange FindTickTimeRange(DateTime time, DateTime sessionStart, TimeSpan tickDuration)
        {
            var timeFromOpening = (time - sessionStart);
            var times = timeFromOpening.Divide(tickDuration);
            if (times.IsIntApprox())
            {
                return new TimeRange(time, time + tickDuration);
            }
            var integerPart = times % 1;
            var tickStart = sessionStart + tickDuration.Multiply(integerPart);
            return new TimeRange(tickStart, tickStart + tickDuration);
        }

        public static TimeRange GetNextTickTimeRange(TimeRange tickTimeRange, TimeSpan tickDuration, TimeRange sessionRange, TimeRange nextSessionRange)
        {
            var result = tickTimeRange.Add(tickDuration);
            return sessionRange.Contains(result)
                ? result
                : new TimeRange(nextSessionRange.Start, nextSessionRange.Start + tickDuration);
        }
    }
}