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
                new PriceEntryFilter{StartTime = 20040101},
                new PriceEntryFilter{EndTime = 20141231},
                new PriceEntryFilter("VOLUME > 200000"),
                new PriceEntryFilter("CLOSE > 10"),
                new PriceEntryFilter("ADJCLOSE > 10"),
            };
        }

        public void LoadPrices()
        {
            //var criteria = BuildCriteria();
            //return Task.Run(() =>
            //{
            //    using (var access = DataAccessFactory.New())
            //    using (var command = access.GetCommonCommand())
            //    {
            //        access.GetCalendar("USA");
            //        // make list of dicts first
            //        var prices = DataCache.PriceCache;

            //        var date = testStart;
            //        while (date <= testEnd)
            //        {
            //            if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday
            //                && !usNonMarketDates.Contains(date.ToDateInt()))
            //                prices[date] = new Dictionary<long, Price>();
            //            date = date.AddDays(1);
            //        }

            //        using (ReportTime.Start())
            //        {
            //            // both end inclusive
            //            for (int year = SelectedStartYear; year <= SelectedEndYear; year++)
            //            {
            //                progressIndicator.SetMessage("Loading " + year);
            //                using (ReportTime.Start(year + " used time: {0}"))
            //                {
            //                    foreach (
            //                        var price in access.GetPrices(year, criteria))
            //                    {
            //                        var secId = price.SecId;
            //                        prices[price.At][secId] = price;
            //                        if (price.At > endOfData)
            //                            endOfData = price.At;
            //                    }
            //                }
            //            }
            //        }



            //        if (command != null)
            //            command.Dispose();
            //    }
            //});
        }

        private string BuildCriteria()
        {
            var result = "";
            foreach (var filter in Filters)
            {
                if (filter.Expression != null)
                {
                    result += filter.Expression + " AND ";
                }
                else if (filter.StartTime != 0)
                {
                    result += "TIME >= " + filter.StartTime + " AND ";
                }
                else if (filter.EndTime != 0)
                {
                    result += "TIME <= " + filter.EndTime + " AND ";
                }
            }
            return result.Trim(" AND ");
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

        public int StartTime { get; set; }
        public int EndTime { get; set; }
    }

    public interface IDataCacheViewModel : IHasViewService, IHasDataAccessFactory<Access>
    {
    }
}