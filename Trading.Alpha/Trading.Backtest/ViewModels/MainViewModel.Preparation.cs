using System;
using System.Collections.Generic;
using System.Data;
using Trading.Common.Entities;
using Trading.Common.Utils;

namespace Trading.Backtest.ViewModels
{
    public partial class MainViewModel
    {
        /// <summary>
        /// c1: the 1000 biggest volume as sec universe.
        /// c2: check secs every monday.
        /// </summary>
        /// <returns></returns>
        private List<Security> PrepareSecurities(DateTime time)
        {
            if (time.DayOfWeek != DayOfWeek.Monday)
                return null;
            DataTable dt;
            using (var access = DataAccessFactory.New())
            {
                dt =access.Query(@"SELECT SECID, CLOSE, VOLUME FROM PRICES P, SECURITIES S WHERE P.SECID = S.ID
AND TIME = '" + time.IsoFormat() + @"'ORDER BY VOLUME DESC LIMIT 0, 1000");
            }
            foreach (DataRow dataRow in dt.Rows)
            {
                Console.WriteLine(dataRow);
            }
        }
    }
}