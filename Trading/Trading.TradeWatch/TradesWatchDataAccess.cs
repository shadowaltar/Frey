using Trading.Common;
using Trading.Common.Data;
using System.Data;
using Trading.Common.Utils;

namespace Trading.TradeWatch
{
    public class TradesScreenDataAccess : TradingDataAccess
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
    }
}