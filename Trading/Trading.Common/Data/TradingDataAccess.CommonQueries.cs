using System;
using System.Collections.Generic;
using System.Data;
using Trading.Common.Entities;
using Trading.Common.Utils;

namespace Trading.Common.Data
{
    public partial class TradingDataAccess
    {
        public IEnumerable<Security> EnumerateAllSecurities(Dictionary<long, Market> markets)
        {
            return YieldQuery(r =>
              {
                  Market mkt;
                  markets.TryGetValue(r["MarketId"].ConvertLong(), out mkt);
                  var security = new Security
                  {
                      Id = r["Id"].ConvertLong(),
                      Code = r["Symbol"].ConvertString(),
                      Name = r["Name"].ConvertString(),
                      Type = r["Type"].ConvertString(),
                      Inception = r["InceptionDate"].ConvertDate(),
                      LotSize = r["Lot"].ConvertInt(),
                      Currency = r["Currency"].ConvertString(),
                      IsShortSellable = r["AllowShortSell"].ConvertInt().From01(),
                      Market = mkt,
                  };
                  DataCache.SecurityCache[security.Id] = security;
                  return security;
              }, "Select * from References.Securities ORDER BY MarketId, Symbol");
        }

        public IEnumerable<Price> EnumeratePrices(Security security, TimeSpan span, DateTime from, DateTime to)
        {
            return YieldQuery(r => new Price
            {
                Security = security,
                At = r["Time"].ConvertDate(),
                Span = span,
                Open = r["Open"].ConvertDouble(),
                High = r["High"].ConvertDouble(),
                Low = r["Low"].ConvertDouble(),
                Close = r["Close"].ConvertDouble(),
                Volume = r["Volume"].ConvertLong(),
            }, "Select * from Instruments.Prices Where SecurityId = {0} and Time >= '{1}' and Time <= '{2}' AND ORDER BY Time",
            security.Id, from.IsoFormat(), to.IsoFormat());
        }

        public DataTable GetAllMarkets()
        {
            return Query("Select * from References.Markets");
        }
    }
}