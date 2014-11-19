using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Documents;
using Trading.Common.Entities;
using Trading.Common.Utils;

namespace Trading.Common.Data
{
    public partial class TradingDataAccess
    {
        public Price GetPrice(long sid, int time)
        {
            var table = Query("SELECT OPEN, HIGH, LOW, CLOSE, VOLUME, ADJCLOSE FROM PRICES WHERE TIME = {0} AND SECID = {1}",
                  time, sid);
            return table.To(PriceConvert).FirstOrDefault();
        }

        public IEnumerable<int> GetHolidays()
        {
            var table = Query("SELECT DATE FROM CALENDAR");
            return table.To(r => r["DATE"].Int());
        }

        public IEnumerable<Security> GetSecurities()
        {
            var table = Query("SELECT ID, CODE, NAME FROM SECURITIES");
            return table.To(SecurityConvert);
        }

        protected Price PriceConvert(DataRow row)
        {
            return new Price
            {
                SecId = row["SECID"].Long(),
                Time = row["TIME"].ConvertDate("yyyyMMdd"),
                Open = row["OPEN"].Double(),
                High = row["HIGH"].Double(),
                Low = row["LOW"].Double(),
                Close = row["CLOSE"].Double(),
                Volume = row["Volume"].Long(),
                AdjClose = row["ADJCLOSE"].Double(),
            };
        }

        protected Security SecurityConvert(DataRow row)
        {
            return new Security
            {
                Id = row["ID"].Long(),
                Code = row["CODE"].String(),
                Name = row["NAME"].String(),
            };
        }
    }
}