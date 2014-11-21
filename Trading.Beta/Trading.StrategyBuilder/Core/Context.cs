using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Common.Data;
using Trading.Common.Entities;
using Trading.Common.Utils;
using Trading.StrategyBuilder.Data;

namespace Trading.StrategyBuilder.Core
{
    public class Context
    {
        public IDataAccessFactory<Access> DataAccessFactory { get; set; }

        public DateTime Start { get { return dataCriteria.Start; } }
        public DateTime End { get { return dataCriteria.End; } }

        private DataCriteria dataCriteria;

        public void Initialize(DataCriteria criteria)
        {
            dataCriteria = criteria;
        }

        public async Task Prepare()
        {
            await Task.WhenAll(LoadPrices(), LoadCalendar(), LoadSecurities());
        }

        public Task LoadCalendar()
        {
            return Task.Run(() =>
            {
                using (ReportTime.Start("Price data fetch used: {0}"))
                using (var access = DataAccessFactory.New())
                {
                    Database.Holidays.AddRange(access.GetHolidays());
                }
            });
        }

        public Task LoadSecurities()
        {
            return Task.Run(() =>
            {
                using (ReportTime.Start("Price data fetch used: {0}"))
                using (var access = DataAccessFactory.New())
                {
                    Database.Securities = access.GetSecurities().ToDictionary(s => s.Id, s => s);
                }
            });
        }

        public Task LoadPrices()
        {
            return Task.Run(() =>
            {
                using (ReportTime.Start("Price data fetch used: {0}"))
                using (var access = DataAccessFactory.New())
                using (var priceTable = access.GetPricesByYear(dataCriteria))
                {
                    Database.Prices = DualDictionary<int, long, Price>.From(priceTable.AsEnumerable(),
                        r => r["TIME"].Int(), r => r["SECID"].Long(),
                        r => new Price
                        {
                            SecId = r[0].Long(),
                            Time = r["TIME"].ConvertDate("yyyyMMdd"),
                            Open = r["OPEN"].Double(),
                            High = r["HIGH"].Double(),
                            Low = r["LOW"].Double(),
                            Close = r["CLOSE"].Double(),
                            AdjClose = r["ADJCLOSE"].Double(),
                            Volume = r["VOLUME"].Long(),
                        });
                }
            });
        }

        private void PushData()
        {
            var prices = DataCache.PriceCache;
            //var today = StartDate;
            //for (int i = 0; i < TotalTradingDays; i++)
            //{
            //    PushData(prices[today]);
            //    today = NextTradingDay(today);
            //}
        }

        protected virtual void OnDataArrived(DateTime time)
        {

        }
    }

    public class DataCriteria : Condition
    {
        public DataCriteria(string timeColumn = "TIME")
        {
            TimeColumn = timeColumn;
        }

        public string ToWhereClauseWithDates()
        {
            var sb = new StringBuilder(ToWhereClause());
            sb.Append(" AND ").Append(TimeColumn).Append(" >= ").Append(Start.ToDateInt())
                .Append(" AND ").Append(TimeColumn).Append(" <= ").Append(End.ToDateInt());
            return sb.ToString();
        }

        public string[] ToWhereClauseSplitByYears()
        {
            var sb = new StringBuilder(ToWhereClause());
            sb.Append(" AND ").Append(TimeColumn).Append(" >= ").Append("{0}")
                .Append(" AND ").Append(TimeColumn).Append(" <= ").Append("{1}");
            var clause = sb.ToString();

            var yearDiff = End.Year - Start.Year;
            var results = new string[yearDiff];
            for (int i = 0; i < yearDiff; i++)
            {
                var start = Start.AddYears(i);
                var end = Start.AddYears(i + 1);
                results[i] = string.Format(clause, start.ToDateInt(), end.ToDateInt());
            }
            return results;
        }

        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string TimeColumn { get; set; }

        public SecurityTestUniverseType SecurityTestUniverseType { get; set; }
    }

    public enum SecurityTestUniverseType
    {
        SelectedSecurities,
        WholeMarket,
    }
}