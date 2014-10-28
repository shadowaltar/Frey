using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Trading.Backtest.Data;
using Trading.Common.Utils;

namespace Trading.Backtest.ViewModels
{
    public partial class MainViewModel
    {
        private Task GetSecurityVolumeInfo()
        {
            var results = DataCache.VolumeCache;
            var startTime = new DateTime(SelectedStartYear, 1, 1);
            var endTime = new DateTime(SelectedEndYear < SelectedStartYear ? SelectedStartYear : SelectedEndYear, 12, 31);
            var currentTime = startTime;
            var periodEndDate = startTime.AddYears(1);
            return Task.Run(() =>
            {
                using (var access = DataAccessFactory.New())
                using (var cmd = new MySqlCommand())
                {
                    while (currentTime < endTime)
                    {
                        using (ReportTime.Start("1Y time: {0}"))
                        {
                            var volumes = GetSecurityVolumeInfo(cmd, access, currentTime, periodEndDate);
                            foreach (var pair in volumes)
                            {
                                results[pair.Key] = pair.Value;
                            }
                            progressIndicator.SetMessage("Year " + currentTime.Year + " is loaded.");
                        }
                        // to the next dates
                        currentTime = periodEndDate.AddDays(1);
                        periodEndDate = currentTime.AddYears(1);
                    }
                }

                Console.WriteLine(results.Count);
            });
        }

        private Task GetAllSecurities()
        {
            var results = DataCache.SecurityCache;
            return Task.Run(() =>
            {
                using (var access = DataAccessFactory.New())
                {

                    results.AddRange(access.GetAllSecurities());
                }

                Console.WriteLine(results.Count);
            });
        }

        /// <summary>
        /// c1: the 1000 biggest volume as sec universe.
        /// c2: check secs every tuesday.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<KeyValuePair<DateTime, Dictionary<int, double>>> GetSecurityVolumeInfo(MySqlCommand cmd,
            BacktestDataAccess access, DateTime time, DateTime time2)
        {
            DataTable dt = access.QueryEx(cmd,
                @"SELECT SECID, date_format(time, '%Y-%m-%d') TIME, VOLUME FROM PRICES P WHERE TIME >= '" + time.IsoFormat()
                + @"' AND TIME <= '" + time2.IsoFormat() + "' AND VOLUME > 200000");

            var ct = dt.FirstOrDefault()["TIME"].ToString();
            var dict = new Dictionary<int, double>();
            foreach (DataRow r in dt.Rows)
            {
                var t = r["TIME"].ToString();
                if (ct != t && dict.Count > 0)
                {
                    yield return new KeyValuePair<DateTime, Dictionary<int, double>>(ct.ConvertDate(), dict);
                    dict = new Dictionary<int, double>();
                }

                ct = t;
                dict[r["SECID"].ConvertInt()] = r["VOLUME"].ConvertDouble();
            }
        }
    }
}