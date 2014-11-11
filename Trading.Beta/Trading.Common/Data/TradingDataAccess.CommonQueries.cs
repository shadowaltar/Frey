using System.Data;
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
            if (table.Rows.Count == 0)
                return null;
            var row = table.FirstOrDefault();
            return new Price
            {
                SecId = sid,
                At = time.ConvertDate("yyyyMMdd"),
                Open = row["OPEN"].Double(),
                High = row["HIGH"].Double(),
                Low = row["LOW"].Double(),
                Close = row["CLOSE"].Double(),
                Volume = row["Volume"].Long(),
                AdjClose = row["ADJCLOSE"].Double(),
            };
        }

        protected Security SecurityConvert(DataRow r)
        {
            return new Security
            {
                Id = r["ID"].Long(),
                Code = r["CODE"].String(),
                Name = r["NAME"].String(),
            };
        }
    }
}