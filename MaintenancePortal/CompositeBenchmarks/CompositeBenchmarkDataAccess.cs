using System.Data;
using Maintenance.Common;
using Maintenance.Common.Data;
using Maintenance.Common.Entities;
using Maintenance.Common.Utils;

namespace Maintenance.CompositeBenchmarks
{
    public class CompositeBenchmarkDataAccess : ImapDataAccess
    {
        public DataTable GetCompositeBenchmarkItems()
        {
            return Query(@"
SELECT COMPOSITE_BENCHMARK_ITEM_ID ID, PORTFOLIO_ASM_LINK_ID LINKID, 
ASM_COMP_ID COMPID, INDEX_ID IID, WEIGHT FROM COMPOSITE_BENCHMARK_ITEM");
        }

        public bool AddPortfolioAssetMapLink(PortfolioAssetMapLink cb)
        {
            var cbId = Query("SELECT SEQ_CB_ID.NEXTVAL FROM DUAL").Rows[0][0].ConvertLong();

            var rowCount = Execute(@"
INSERT INTO PORTFOLIO_ASM_LINK 
(PORTFOLIO_ASM_LINK_ID, PORTFOLIO_ID, ASM_ID, EFFECTIVE_DT, EXPIRATION_DT, 
CREATED_BY, CREATE_TM, UPDATED_BY, UPDATE_TM) 
VALUES 
({0}, {1}, {2}, TO_DATE('{3}','YYYY-MM-DD'), TO_DATE('{4}','YYYY-MM-DD'), '{5}', SYSDATE, '{6}', SYSDATE)"
                , cbId, cb.Portfolio.Id, cb.AssetMap.Id,
                cb.EffectiveDate.IsoFormat(), cb.ExpiryDate.IsoFormat(),
                Constants.UserName, Constants.UserName);

            if (rowCount == 1)
            {
                cb.Id = cbId;
                return true;
            }
            return false;
        }

        public bool AddCompositeBenchmarkItem(CompositeBenchmarkItem item)
        {
            var rowCount = Execute(@"
INSERT INTO COMPOSITE_BENCHMARK_ITEM 
(COMPOSITE_BENCHMARK_ITEM_ID, PORTFOLIO_ASM_LINK_ID, ASM_COMP_ID, INDEX_ID, WEIGHT) 
VALUES (SEQ_CB_ITEM_ID.NEXTVAL, {0}, {1}, {2}, {3})"
                , item.PortfolioAssetMapLink.Id,
                item.AssetMapComponent.Id, item.Index.Id, item.Weight);
            return rowCount == 1;
        }

        public bool DeleteCompositeBenchmarkItem(CompositeBenchmarkItem item)
        {
            var rowCount = Execute(@"
DELETE COMPOSITE_BENCHMARK_ITEM WHERE COMPOSITE_BENCHMARK_ITEM_ID = {0}", item.Id);
            return rowCount == 1;
        }

        public bool ModifyCompositeBenchmarkItem(CompositeBenchmarkItem item)
        {
            var rowCount = Execute(@"
UPDATE COMPOSITE_BENCHMARK_ITEM SET INDEX_ID = {0}, WEIGHT = {1} WHERE COMPOSITE_BENCHMARK_ITEM_ID = {2}", item.Index.Id, item.Weight, item.Id);
            return rowCount == 1;
        }

        public DataTable GetLastEffectivePortfolioAssetMapLink(long pid, long aid)
        {
            return Query(@"
SELECT L.ASM_ID AS AID, L.PORTFOLIO_ID AS PID, TO_CHAR(L.EFFECTIVE_DT,'YYYY-MM-DD') AS EFFECTIVE, 
TO_CHAR(L.EXPIRATION_DT,'YYYY-MM-DD') AS EXPIRY, L.PORTFOLIO_ASM_LINK_ID AS ID 
FROM PORTFOLIO_ASM_LINK L 
JOIN 
(SELECT PORTFOLIO_ID, ASM_ID, MAX(EFFECTIVE_DT) AS EFFECTIVE 
FROM PORTFOLIO_ASM_LINK WHERE PORTFOLIO_ID = {0} AND ASM_ID = {1} GROUP BY (PORTFOLIO_ID, ASM_ID)) T 
ON T.PORTFOLIO_ID = L.PORTFOLIO_ID AND T.ASM_ID = L.ASM_ID AND T.EFFECTIVE = L.EFFECTIVE_DT"
                , pid, aid);
        }

        public bool UpdatePortfolioAssetMapLink(PortfolioAssetMapLink cb)
        {
            cb.ThrowIfNull("cb", "Must provide a composite benchmark to be updated.");
            var rowCount = Execute(@"
UPDATE PORTFOLIO_ASM_LINK SET IS_DEFAULT = '{0}', EFFECTIVE_DT = TO_DATE('{1}','YYYY-MM-DD'), 
EXPIRATION_DT = TO_DATE('{2}','YYYY-MM-DD'), UPDATED_BY = '{3}', 
UPDATE_TM = SYSDATE WHERE PORTFOLIO_ASM_LINK_ID = {4} AND PORTFOLIO_ID = {5} AND ASM_ID = {6}"
                , cb.IsDefault ? "Y" : "N", cb.EffectiveDate.IsoFormat(), cb.ExpiryDate.IsoFormat(),
                Constants.UserName, cb.Id, cb.Portfolio.Id, cb.AssetMap.Id);
            return rowCount == 1;
        }
    }
}