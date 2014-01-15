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

            if (!Directory.Exists(StaticDataFileDirectory))
                Directory.CreateDirectory(StaticDataFileDirectory);
            if (!Directory.Exists(LocalTempFileDirectory))
                Directory.CreateDirectory(LocalTempFileDirectory);
        }

        public static string StaticDataFileDirectory { get { return "../../../../StaticDataFiles"; } }
        public static string LocalTempFileDirectory { get { return "../../../../TempFiles"; } }

        public static IReferenceUniverse References { get; private set; }
        public const double DoubleTolerance = 1E-8;

        public static IEnumerable<Price> Convert(IEnumerable<string[]> data, Security security,
            TimeSpan duration,
            PriceSourceType priceSourceType = PriceSourceType.Unknown,
            TimeZoneType priceTimeZoneType = TimeZoneType.UTC)
        {
            switch (priceSourceType)
            {
                case PriceSourceType.YahooHistorical:
                    foreach (var datum in data)
                    {
                        var price = new Price
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
                        if (priceTimeZoneType == TimeZoneType.AmericaTime)
                            price.Time = price.Time.AmericaToUTC0();
                        yield return price;
                    }
                    break;
                case PriceSourceType.DailyFXHistorical:
                    foreach (var x in data)
                    {
                        var price = new Price
                        {
                            Time = (x[0] + x[1]).ToDateTime("M/d/yyyyHH:mm:ss"),
                            Duration = duration,

                            Open = x[2].ToDouble(),
                            High = x[3].ToDouble(),
                            Low = x[4].ToDouble(),
                            Close = x[5].ToDouble(),

                            Security = security,
                        };
                        if (priceTimeZoneType == TimeZoneType.AmericaTime)
                            price.Time = price.Time.AmericaToUTC0();
                        yield return price;
                    }
                    break;
            }
        }
    }
}