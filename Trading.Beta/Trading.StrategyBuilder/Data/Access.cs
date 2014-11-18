using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Trading.Common.Data;
using Trading.Common.Entities;
using Trading.Common.Utils;
using Trading.StrategyBuilder.Core;

namespace Trading.StrategyBuilder.Data
{
    public class Access : TradingDataAccess
    {
        public List<Security> FindSecurity(string partialSecurityInfo)
        {
            if (string.IsNullOrWhiteSpace(partialSecurityInfo) || partialSecurityInfo.Trim().Length < -3)
                return null;
            partialSecurityInfo = partialSecurityInfo.ToUpperInvariant();

            var table = Query("SELECT * FROM SECURITIES WHERE CODE LIKE '%{0}%' OR UPPER(NAME) LIKE '%{0}%'", partialSecurityInfo);
            return table.To(SecurityConvert).OrderBy(s => s.Code).ToList();
        }

        public DualDictionary<DateTime, long, Price> GetPrices(DataCriteria dataCriteria)
        {
            return Task.Run(() =>
            {

                using (var commonCommand = GetCommand())
                {
                    
                }
                // make list of dicts first
                prices = DataCache.PriceCache;
                invertedPrices = DataCache.InvertedPriceCache;
                prices.Clear();
                invertedPrices.Clear();

                var date = testStart;
                while (date <= testEnd)
                {
                    if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday
                        && !usNonMarketDates.Contains(date.ToDateInt()))
                        prices[date] = new Dictionary<long, Price>();
                    date = date.AddDays(1);
                }

                using (ReportTime.Start())
                {
                    // both end inclusive
                    for (int year = SelectedStartYear; year <= SelectedEndYear; year++)
                    {
                        progressIndicator.SetMessage("Loading " + year);
                        using (ReportTime.Start(year + " used time: {0}"))
                        {
                            foreach (var price in commonAccess.GetOneYearPriceData(year, core.GetDataCriteriaInSql()))
                            {
                                var secId = price.SecId;
                                if (!prices.ContainsKey(price.At))
                                {
                                    prices[price.At] = new Dictionary<long, Price>();
                                }
                                //if (!invertedPrices.ContainsKey(secId))
                                //{
                                //    invertedPrices[secId] = new Dictionary<DateTime, Price>();
                                //}
                                prices[price.At][secId] = price;
                                //        invertedPrices[secId][price.At] = price;
                                if (price.At > endOfData)
                                    endOfData = price.At;
                            }
                        }
                    }
                }


                if (commonAccess != null)
                    commonAccess.Dispose();
                if (commonCommand != null)
                    commonCommand.Dispose();
            });
        }
    }
}