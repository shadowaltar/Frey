using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using Trading.Backtest.Data;
using Trading.Common.Utils;

namespace Trading.Backtest.ViewModels
{
    public partial class MainViewModel
    {
        /// <summary>
        /// c1: the 1000 biggest volume as sec universe.
        /// c2: check secs every tuesday.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<KeyValuePair<DateTime, Dictionary<int, double>>> PrepareSecurities(MySqlCommand cmd,
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