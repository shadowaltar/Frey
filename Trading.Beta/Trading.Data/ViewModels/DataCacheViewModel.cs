using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Caliburn.Micro;
using PropertyChanged;
using Trading.Common.Data;
using Trading.Common.Entities;
using Trading.Common.Utils;
using Trading.Common.ViewModels;
using Trading.Data.Data;

namespace Trading.Data.ViewModels
{
    [ImplementPropertyChanged]
    public class DataCacheViewModel : ViewModelBase, IDataCacheViewModel
    {
        private static string DefaultSchema { get { return "TRADING"; } }
        public IViewService ViewService { get; set; }
        public IDataAccessFactory<Access> DataAccessFactory { get; set; }

        public BindableCollection<PriceEntryFilter> Filters { get; private set; }

        public DataCacheViewModel()
        {
            Filters = new BindableCollection<PriceEntryFilter>
            {
                new PriceEntryFilter("VOLUME > 200000"),
                new PriceEntryFilter("CLOSE > 10"),
                new PriceEntryFilter("ADJCLOSE > 10"),
            };
        }

        public void LoadPrices()
        {
            return Task.Run(() =>
            {
                Access commonAccess =null;
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

        public Task LoadSecurities()
        {
            var results = DataCache.SecurityCache;
            var map = DataCache.SecurityCodeMap;
            return Task.Run(() =>
            {
                using (ReportTime.Start("Get all securities used {0}"))
                using (var access = DataAccessFactory.New())
                {
                    var secs = access.GetSecurities();
                    foreach (var sec in secs)
                    {
                        results[sec.Id] = sec;
                        map[sec.Code] = sec.Id;
                    }
                }
            });
        }
    }

    public class PriceEntryFilter
    {
        public PriceEntryFilter()
        {
        }

        public PriceEntryFilter(string expression)
        {
            Expression = expression;
        }

        public string Expression { get; set; }
    }

    public interface IDataCacheViewModel : IHasViewService, IHasDataAccessFactory<Access>
    {
    }
}