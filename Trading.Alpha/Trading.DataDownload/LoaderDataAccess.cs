using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Common.Data;
using Trading.Common.Entities;
using Trading.Common.Utils;

namespace Trading.DataDownload
{
    public class LoaderDataAccess : TradingDataAccess, ILoaderDataAccess
    {
        public DataTable GetMarketId(string marketCode)
        {
            return Query("SELECT Id FROM ReferenceData.Markets WHERE Symbol = '{0}'", marketCode);
        }

        public bool DeleteSecurities(int marketId)
        {
            return Execute(@"DELETE FROM SecurityData.Securities WHERE MarketId = {0} ", marketId) >= 1;
        }

        public void DeleteSecurity(string securityCode, int marketId)
        {
            Execute(@"DELETE FROM SecurityData.Securities WHERE MarketId = {0} AND Code = '{1}'", marketId, securityCode);
        }

        public decimal CreateStock(Stock security, string marketCode, int mid)
        {
            return Execute(
@"INSERT INTO `SecurityData`.`Securities`
(`Code`,
`Name`,
`MarketId`,
`SectorIndustryId`)
VALUES
('{0}',
'{1}',
{2},
(SELECT Id FROM ReferenceData.SectorIndustry WHERE Type='{3}' AND Sector='{4}' AND Industry='{5}'))"
, security.Code, security.Name, mid, marketCode, security.Sector, security.Industry);
        }

        public bool DeleteSectorIndustries(string type)
        {
            return Execute(@"DELETE FROM ReferenceData.SectorIndustry WHERE Type='{0}'", type) >= 1;
        }

        public bool CreateSectorIndustry(string type, string sector, string industry)
        {
            return Execute(
                @"INSERT INTO ReferenceData.SectorIndustry (Type, Sector, Industry) Values ('{0}','{1}','{2}')",
                type, sector, industry) == 1;
        }

        public bool InsertPrice(int sid, DateTime date, decimal open, decimal high, decimal low, decimal close, decimal volume, decimal adjClose)
        {
            return Execute(@"INSERT INTO `SecurityData`.`SecurityPrices`
(`SecurityId`,
`DateTime`,
`Open`,
`High`,
`Low`,
`Close`,
`AdjustedClose`,
`Volume`)
VALUES
({0},'{1}',{2},{3},{4},{5},{6},{7})"
, sid, date.IsoFormat(), open, high, low, close, adjClose, volume) == 1;
        }

        public void DeleteSecurityPrices(int sid)
        {
            Execute(@"DELETE `SecurityData`.`SecurityPrices` WHERE `SecurityId` = {0}", sid);
        }
    }

    public interface ILoaderDataAccess
    {
    }
}
