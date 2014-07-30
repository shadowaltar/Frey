using Maintenance.Common;
using Maintenance.Common.Data;
using System.Data;
using Maintenance.Common.Utils;
using Maintenance.Portfolios.Entities;

namespace Maintenance.Portfolios
{
    public class PortfolioDataAccess : ImapDataAccess
    {
        public DataTable GetInstrumentAssetClassFocuses()
        {
            return Query(@"
SELECT DISTINCT 
I.INSTRUMENT_ID ID, I.INSTRUMENT_SHORT_NAME NAME, I.ASSET_CLASS_FOCUS CLASS, 
P.PORTFOLIO_ID PID FROM PORTFOLIO P 
JOIN INSTRUMENT I 
ON I.RELATED_PORTFOLIO_ID = P.PORTFOLIO_ID 
AND I.ASSET_CLASS_FOCUS IS NOT NULL");
        }

        public DataTable GetAllSectorSchemes()
        {
            return Query(@"SELECT SECTOR_SCHEME_CD CODE FROM SECTOR_SCHEME ORDER BY SECTOR_SCHEME_CD");
        }

        public DataTable GetInstruments()
        {
            return Query(@"
SELECT DISTINCT 
I.INSTRUMENT_ID ID,
I.INSTRUMENT_SHORT_NAME NAME,
ASSET_CLASS_FOCUS CLASS,
P.PORTFOLIO_ID PID
FROM INSTRUMENT I
LEFT JOIN MDM_FIL.PORTFOLIO P
ON I.RELATED_PORTFOLIO_ID = P.PORTFOLIO_ID
LEFT OUTER JOIN MDM_FIL.PORTFOLIO_PERSON PP
ON PP.PORTFOLIO_ID  = P.PORTFOLIO_ID
LEFT OUTER JOIN MDM_FIL.PERSON PE
ON PE.PERSON_ID = PP.PERSON_ID
WHERE PE.PERSON_STATUS_CD   != 'INACTIVE'
AND P.PORTFOLIO_STATUS_CD != 'INACTIVE'
AND P.DEACTIVATION_DT  IS NULL
AND PE.DEACTIVATION_DT IS NULL
AND PP.PRIMARY_ROLE_IND = 'Y'");
        }

        public DataTable GetAllBenchmarks()
        {
            return Query(@"
SELECT
PORTFOLIO_ID PID,
INDEX_ID IID,
EFFECTIVE_DT EFFECTIVE,
EXPIRATION_DT EXPIRY,
BENCHMARK_TYPE_CD TYPE
FROM PORTFOLIO_INDEX_BENCHMARK");
        }

        public DataTable GetAssetMapComponents()
        {
            return Query(@"SELECT * FROM ASM_COMPONENT");
        }

        public DataTable GetAssetMapRelationship()
        {
            return Query(@"SELECT * FROM PORTFOLIO_ASM_MAP");
        }

        public DataTable GetAssetMaps()
        {
            return Query(@"SELECT ASM_ID ID, ASM_NAME NAME, ASM_CODE CODE FROM ASM_STRUCTURE");
        }

        public DataTable GetAllPortfolioExtendedInfo()
        {
            return Query(@"
SELECT 
PORTFOLIO_ID ID, TLF_FLAG IS_TOP_LEVEL_FUND,
SECTOR_SCHEME_CD SECTOR_SCHEME, ASSET_CLASS_FOCUS, LOCATION_CD LOCATION
FROM IMAP_COMMON.PORTFOLIO_EXT");
        }

        public bool InsertBenchmark(long pid, Benchmark benchmark)
        {
            var rowCount = Execute(@"
INSERT INTO 
IMAP_FODE.PORTFOLIO_INDEX_BENCHMARK_AP 
(PORTFOLIO_ID, BENCHMARK_TYPE_CD, EFFECTIVE_DT, EXPIRATION_DT, INDEX_ID, CREATED_BY, CREATE_TM, UPDATED_BY, UPDATE_TM)
VALUES 
({0}, 'IMAP', TO_DATE('{1}','YYYY-MM-DD'), TO_DATE('{2}','YYYY-MM-DD'), {3}, '{4}', SYSDATE, '{5}', SYSDATE)"
                , pid, benchmark.EffectiveDate.IsoFormat(), benchmark.ExpiryDate.IsoFormat(),
                benchmark.Index.Id, Constants.UserName, Constants.UserName);

            return rowCount == 1;
        }

        public bool UpdateBenchmark(long pid,
            Benchmark benchmark, Benchmark oldBenchmark)
        {
            var rowCount = Execute(@"
UPDATE
IMAP_FODE.PORTFOLIO_INDEX_BENCHMARK_AP
SET 
INDEX_ID = {0}, 
EFFECTIVE_DT = TO_DATE('{1}','YYYY-MM-DD'), EXPIRATION_DT = TO_DATE('{2}','YYYY-MM-DD'),
UPDATED_BY = '{3}', UPDATE_TM = SYSDATE
WHERE 
PORTFOLIO_ID = {4} AND INDEX_ID = {5}
AND EFFECTIVE_DT = TO_DATE('{6}','YYYY-MM-DD') AND EXPIRATION_DT = TO_DATE('{7}','YYYY-MM-DD')"
                  , benchmark.Index.Id,
                  benchmark.EffectiveDate.IsoFormat(), benchmark.ExpiryDate.IsoFormat(), Constants.UserName,
                  pid, oldBenchmark.Index.Id,
                  oldBenchmark.EffectiveDate.IsoFormat(), oldBenchmark.ExpiryDate.IsoFormat());
            return rowCount == 1;
        }

        public bool DeleteBenchmark(long pid, Benchmark oldBenchmark)
        {
            var rowCount = Execute(@"
DELETE FROM 
IMAP_FODE.PORTFOLIO_INDEX_BENCHMARK_AP 
WHERE
PORTFOLIO_ID = {0} AND INDEX_ID = {1}
AND EFFECTIVE_DT = TO_DATE('{2}','YYYY-MM-DD') 
AND EXPIRATION_DT = TO_DATE('{3}','YYYY-MM-DD')"
                , pid, oldBenchmark.Index.Id,
                oldBenchmark.EffectiveDate.IsoFormat(), oldBenchmark.ExpiryDate.IsoFormat());
            return rowCount == 1;
        }

        public bool InsertPortfolioExtendedInfo(long pid, PortfolioExtendedInfo info)
        {
            var rowCount = Execute(@"
INSERT INTO 
IMAP_COMMON.PORTFOLIO_EXT 
(PORTFOLIO_ID,
QFII_STATUS,
TLF_FLAG,
CREATE_DT,
CREATED_BY,
UPDATE_DT,
UPDATED_BY,
SECTOR_SCHEME_CD,
ASSET_CLASS_FOCUS,
LOCATION_CD)
VALUES 
({0},0,'{1}',SYSDATE,'{2}',SYSDATE,'{2}',{3},{4},{5})"
                , pid, info.IsTopLevelFund ? "Y" : "N"
                , Constants.UserName,
                info.SectorScheme == null ? "NULL" : info.SectorScheme.QuoteMe(),
                info.AssetClassFocus == null ? "NULL" : info.AssetClassFocus.QuoteMe(),
                info.Location == null ? "NULL" : info.Location.Code.QuoteMe());
            return rowCount == 1;
        }

        public bool UpdatePortfolioExtendedInfo(long pid, PortfolioExtendedInfo info)
        {
            var rowCount = Execute(@"
UPDATE IMAP_COMMON.PORTFOLIO_EXT
SET TLF_FLAG = '{0}',
SECTOR_SCHEME_CD = {1}, ASSET_CLASS_FOCUS = {2},
LOCATION_CD = {3},
UPDATE_DT = SYSDATE, UPDATED_BY = '{4}'
WHERE
PORTFOLIO_ID = {5}"
                , info.IsTopLevelFund ? "Y" : "N",
                info.SectorScheme == null ? "NULL" : info.SectorScheme.QuoteMe(),
                info.AssetClassFocus == null ? "NULL" : info.AssetClassFocus.QuoteMe(),
                info.Location == null ? "NULL" : info.Location.Code.QuoteMe(),
                Constants.UserName, pid);
            return rowCount == 1;
        }
    }
}