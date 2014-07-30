using System.Data;

namespace Maintenance.Common.Data
{
    public partial class ImapDataAccess
    {
        /// <summary>
        /// Get all portfolios and their managers.
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllPortfoliosAndManagers()
        {
            return Query(@"SELECT P.PORTFOLIO_ID PID, P.PORTFOLIO_SHORT_NAME CODE, P.PORTFOLIO_LONG_NAME NAME, PP.PERSON_ID PMID, PP.PERSON_ALT_ID PMAID, PP.DISPLAY_NAME PMNAME FROM PORTFOLIO P LEFT OUTER JOIN PERSON PP ON PP.PERSON_ID = P.PORTFOLIO_MANAGER_ID");
        }

        /// <summary>
        /// Get all indexes.
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllIndexes()
        {
            return Query(@"SELECT INDEX_ID ID, INDEX_CD CODE, INDEX_LONG_NAME NAME, DEACTIVATION_DATE EXPIRY FROM INDEX_BASIC");
        }

        /// <summary>
        /// Get all countries (locations with type "Country").
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllCountries()
        {
            return Query(@"SELECT LOCATION_CD CODE, LOCATION_DESC NAME, DEACTIVATION_DATE EXPIRY FROM COUNTRY");
        }

        /// <summary>
        /// Get all asset maps' components.
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllAssetMapComponents()
        {
            return Query(@"SELECT ASM_COMP_ID ID, ASM_COMP_CODE CODE, ASM_COMP_NAME NAME, ASM_COMP_ORDER ""ORDER"", ASM_ID AID, PARENT_ASM_COMP_ID PARENTID FROM ASM_COMPONENT");
        }

        /// <summary>
        /// Get all asset maps.
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllAssetMaps()
        {
            return Query(@"SELECT ASM_ID ID, ASM_NAME NAME, ASM_CODE CODE FROM ASM_STRUCTURE");
        }

        /// <summary>
        /// Get all portfolio to asset map links; it is also used to uniquely identify a 'composite benchmark'.
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllPortfolioAssetMapLinks()
        {
            return Query(@"SELECT PORTFOLIO_ASM_LINK_ID LINKID, PORTFOLIO_ID PID, ASM_ID ASMID, IS_DEFAULT ISDEFAULT, TO_CHAR(EFFECTIVE_DT, 'YYYY-MM-DD') EFFECTIVE, TO_CHAR(EXPIRATION_DT, 'YYYY-MM-DD') EXPIRY, CREATED_BY CREATOR, TO_CHAR(CREATE_TM, 'YYYY-MM-DD HH24:MI:SS') CREATETIME, UPDATED_BY UPDATER, TO_CHAR(UPDATE_TM, 'YYYY-MM-DD HH24:MI:SS') UPDATETIME FROM PORTFOLIO_ASM_LINK");
        }
    }
}