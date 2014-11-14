using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trading.Backtest.Data;
using Trading.Common.Entities;
using Trading.Common.Utils;

namespace Trading.Backtest.ViewModels
{
    public partial class MainViewModel
    {
        public async void Initialize()
        {
            core = new Core(10000);
            endOfData = DateTime.MinValue;
            testStart = new DateTime(SelectedStartYear, 1, 1);
            testEnd = new DateTime(SelectedEndYear, 12, 31);

            // get prices 
            progressIndicator = await ViewService.ShowProgress("Loading", "Loading Data..", true);
            await Task.WhenAll(GetAllSecurities(), Task.Run(() =>
            {
                usNonMarketDates.Clear();
                using (var access = DataAccessFactory.New())
                usNonMarketDates.AddRange(access.GetUsMarketClosedDates());
            }));

            progressIndicator.SetMessage("Loading Prices..");
            // get prices from db
            await GetPrices();
            await progressIndicator.Stop();
        }

        private Task GetAllSecurities()
        {
            var results = DataCache.SecurityCache;
            var map = DataCache.SecurityCodeMap;
            return Task.Run(() =>
            {
                using (ReportTime.Start("Get all securities used {0}"))
                using (var access = DataAccessFactory.New())
                {
                    var secs = access.GetAllSecurities();
                    foreach (var sec in secs)
                    {
                        results[sec.Id] = sec;
                        map[sec.Code] = sec.Id;
                    }
                }
            });
        }

        private Task GetPrices()
        {
            return Task.Run(() =>
            {
                if (commonAccess == null)
                    commonAccess = DataAccessFactory.New();
                if (commonCommand == null)
                    commonCommand = commonAccess.GetCommonCommand();

                // make list of dicts first
                prices = DataCache.PriceCache;
                prices.Clear();

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
                                prices[price.At][secId] = price;
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