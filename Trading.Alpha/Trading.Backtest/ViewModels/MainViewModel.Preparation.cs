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
            core = new Core { PortfolioAmount = 10000, Positions = new HashSet<Position>() };
            endOfData = DateTime.MinValue;
            testStart = new DateTime(SelectedStartYear, 1, 1);
            testEnd = new DateTime(SelectedEndYear, 12, 31);

            // get prices 
            progressIndicator = await ViewService.ShowProgress("Loading", "Loading Securities..", true);
            await GetAllSecurities();

            progressIndicator.SetMessage("Loading Prices..");
            // get prices from db
            await GetPrices();
            await progressIndicator.Stop();
        }

        private Task GetAllSecurities()
        {
            var results = DataCache.SecurityCache;
            return Task.Run(() =>
            {
                using (ReportTime.Start("Get all securities used {0}"))
                using (var access = DataAccessFactory.New())
                {
                    var secs = access.GetAllSecurities();
                    foreach (var sec in secs)
                    {
                        results[sec.Id] = sec;
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
                while (date < testEnd)
                {
                    if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                        prices[date] = new Dictionary<long, Price>();
                    date = date.AddDays(1);
                }

                using (ReportTime.Start())
                {
                    for (int year = SelectedStartYear; year <= SelectedEndYear; year++)
                    {
                        progressIndicator.SetProgress((double)(year - SelectedStartYear) /
                                                      (SelectedEndYear - SelectedStartYear));
                        using (ReportTime.Start(year + " used time: {0}"))
                        {
                            foreach (var price in commonAccess.GetOneYearPriceData(year))
                            {
                                var secId = price.SecId;
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