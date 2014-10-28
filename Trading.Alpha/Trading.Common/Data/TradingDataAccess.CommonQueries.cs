using System;
using System.Collections.Generic;
using System.Data;
using Trading.Common.Entities;
using Trading.Common.Utils;

namespace Trading.Common.Data
{
    public partial class TradingDataAccess
    {
        public IEnumerable<Price> EnumeratePrices(Security security, TimeSpan span, DateTime from, DateTime to)
        {
            return YieldQuery(r => new Price
            {
                Security = security,
                At = r["Time"].ConvertDate(),
                Span = span,
                Open = r["Open"].ConvertDecimal(),
                High = r["High"].ConvertDecimal(),
                Low = r["Low"].ConvertDecimal(),
                Close = r["Close"].ConvertDecimal(),
                Volume = r["Volume"].ConvertLong(),
            }, "Select * from Instruments.Prices Where SecurityId = {0} and Time >= '{1}' and Time <= '{2}' ORDER BY Time",
            security.Id, from.IsoFormat(), to.IsoFormat());
        }

        public List<Price> GetPrices(Security security, TimeSpan span, DateTime from, DateTime to)
        {
            var table = Query("Select * from Instruments.Prices Where SecurityId = {0} and Time >= '{1}' and Time <= '{2}' ORDER BY Time",
            security.Id, from.IsoFormat(), to.IsoFormat());
            var results = new List<Price>();
            foreach (DataRow r in table.Rows)
            {
                var p = new Price
                {
                    Security = security,
                    At = r["Time"].ConvertDate(),
                    Span = span,
                    Open = r["Open"].ConvertDecimal(),
                    High = r["High"].ConvertDecimal(),
                    Low = r["Low"].ConvertDecimal(),
                    Close = r["Close"].ConvertDecimal(),
                    Volume = r["Volume"].ConvertLong(),
                };
                results.Add(p);
            }
            return results;
        }

        public PriceSeries GetPrices(long securityId, DateTime from, DateTime to)
        {
            var table = Query("Select DateTime, Open, High, Low, Close, Volume from SecurityData.SecurityPrices Where SecurityId = {0} and DateTime >= '{1}' and DateTime <= '{2}' ORDER BY DateTime",
            securityId, from.IsoFormat(), to.IsoFormat());
            var results = new PriceSeries();
            foreach (DataRow r in table.Rows)
            {
                var p = new PriceBar
                {
                    DateTime = r["DateTime"].ConvertDate(),
                    Open = r["Open"].ConvertDouble(),
                    High = r["High"].ConvertDouble(),
                    Low = r["Low"].ConvertDouble(),
                    Close = r["Close"].ConvertDouble(),
                    Volume = r["Volume"].ConvertLong(),
                };
                results.Add(p);
            }
            return results;
        }

        public DataTable GetAllMarkets()
        {
            return Query("Select * from References.Markets");
        }

        public int GetSecurityId(string securityCode, string marketCode)
        {
            var table = Query(@"SELECT s.Id FROM SecurityData.Securities s
Join ReferenceData.Markets m on m.Id = s.MarketId
WHERE s.Code = '{0}' and m.Symbol = '{1}'", securityCode, marketCode);
            try
            {
                return table.Rows[0][0].ConvertInt(-1);
            }
            catch
            {
                return -1;
            }
        }
    }
}