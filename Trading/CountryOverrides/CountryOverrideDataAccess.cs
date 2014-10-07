using Trading.Common;
using Trading.Common.Data;
using Trading.Common.Entities;
using Trading.CountryOverrides.Entities;
using Trading.CountryOverrides.Utils;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Trading.CountryOverrides
{
    public class CountryOverrideDataAccess : ImapDataAccess
    {
        public bool Delete(OverrideItem overrideItem)
        {
            if (overrideItem == null || overrideItem.Id == 0L)
                return false;
            switch (overrideItem.Type)
            {
                case OverrideType.ALL:
                    {
                        var rowCount =
                            Execute("delete from IMAP_common.all_country_override where tradable_ent_id = " +
                                           overrideItem.Id);
                        return rowCount > 0;
                    }
                case OverrideType.PM:
                    {
                        if (overrideItem.PortfolioManager == null)
                            return false;
                        var rowCount =
                            Execute("delete from IMAP_common.pm_country_override where tradable_ent_id = " +
                                           overrideItem.Id + " and portfolio_manager_id = " + overrideItem.PortfolioManagerId);
                        return rowCount > 0;
                    }
                case OverrideType.PORTFOLIO:
                    {
                        if (overrideItem.Portfolio == null)
                            return false;

                        var rowCount =
                            Execute("delete from IMAP_common.portfolio_country_override where tradable_ent_id = " +
                                           overrideItem.Id + " and portfolio_id = " + overrideItem.PortfolioId);
                        return rowCount > 0;
                    }
            }
            // case "FILC" is invalid.
            throw new InvalidOverrideTypeException(overrideItem.Type);
        }

        public DataTable GetPortfoliosAndManagers(List<string> predefinedPortfolioCodes)
        {
            if (predefinedPortfolioCodes.Count == 0)
                return null;
            var whereClause = predefinedPortfolioCodes.Aggregate(" WHERE portfolio_short_name IN (", (s, code) => s + "'" + code + "',").Trim(',') + ")";
            return Query(@"
SELECT PORTFOLIO_SHORT_NAME CODE, 
PORTFOLIO_LONG_NAME NAME, PORTFOLIO_ID PID, PERSON_ID PMID, DISPLAY_NAME PMNAME 
FROM PORTFOLIO PTF 
JOIN PERSON PM 
ON PTF.PORTFOLIO_MANAGER_ID = PM.PERSON_ID " + whereClause);
        }

        public DataTable GetPortfolios(IEnumerable<long> pIds)
        {
            if (pIds == null)
                return null;

            var whereClause = pIds.Aggregate(string.Empty, (current, id) => current + id + ",").Trim(',');
            if (whereClause.Length == 0)
                return null;

            whereClause = " WHERE portfolio_id IN (" + whereClause + ")";
            return Query("select portfolio_id Id, fund_short_name Code, fund_name Name from FODE_REF_EXTN.portfolio_attributes " + whereClause);
        }

        public DataTable GetPortfolioManagers(IEnumerable<long> pmIds)
        {
            if (pmIds == null)
                return null;

            var whereClause = pmIds.Aggregate(string.Empty, (current, id) => current + id + ",").Trim(',');
            if (whereClause.Length == 0)
                return null;

            whereClause = " WHERE person_id IN (" + whereClause + ")";
            return Query("select person_id Id, display_name Name from FODE_REF_EXTN.person " + whereClause);
        }

        public DataTable GetAllLocationExtensions()
        {
            return Query(@"
SELECT 
FMR_LOCATION_CD CODE_FMR,
LOCATION_CD CODE,
LOCATION_DESC NAME
FROM IMAP_COMMON.LOCATION_EXT
WHERE LOCATION_TYPE_CD = 'COUNTRY'");
        }

        /// <summary>
        /// todo: lacks custom location/country logic as not impl in mt yet.
        /// </summary>
        /// <param name="access"></param>
        /// <returns></returns>
        public DataTable GetOverrides()
        {
            return Query(
@"SELECT X.ID,
    TE.FMRCUSIP CUSIP,
    TE.SEDOL SEDOL,
    I.INSTRUMENT_SHORT_NAME NAME,
    I.INSTRUMENT_LONG_NAME FULLNAME,
    X.NEWCOUNTRYCODE,
    TE.LOCATION_CD OLDCOUNTRYCODE,
    X.TYPE,
    X.PMID,
    X.PORTFOLIOID
FROM (
  (SELECT FCO.TRADABLE_ENT_ID ID,
    FCO.LOCATION_CD NEWCOUNTRYCODE,
    'FILC' TYPE,
    0 PMID,
    0 PORTFOLIOID
  FROM FILC_COUNTRY_OVERRIDE FCO)
UNION
  (SELECT A.TRADABLE_ENT_ID ID,
    A.LOCATION_CD NEWCOUNTRYCODE,
    'ALL' TYPE,
    0 PMID,
    0 PORTFOLIOID
  FROM IMAP_COMMON.ALL_COUNTRY_OVERRIDE A)
UNION
  (SELECT P.TRADABLE_ENT_ID ID,
    P.LOCATION_CD NEWCOUNTRYCODE,
    'PM' TYPE,
    P.PORTFOLIO_MANAGER_ID PMID,
    0 PORTFOLIOID
  FROM IMAP_COMMON.PM_COUNTRY_OVERRIDE P)
UNION
  (SELECT P.TRADABLE_ENT_ID ID,
    P.LOCATION_CD NEWCOUNTRYCODE,
    'PORTFOLIO' TYPE,
    0 PMID,
    P.PORTFOLIO_ID PORTFOLIOID
  FROM IMAP_COMMON.PORTFOLIO_COUNTRY_OVERRIDE P
  ) ) X
JOIN TRADABLE_ENT TE
  ON X.ID = TE.TRADABLE_ENT_ID
JOIN INSTRUMENT I
  ON TE.INSTRUMENT_ID = I.INSTRUMENT_ID
ORDER BY X.ID, X.TYPE");
        }

        public DataTable Search(string partOfInstrumentName, string cusip, string sedol)
        {
            var whereClause = string.Empty;
            if (!string.IsNullOrEmpty(cusip))
            {
                whereClause = "FMRCUSIP = '" + cusip + "'";
            }
            else if (!string.IsNullOrEmpty(sedol))
            {
                whereClause = "SEDOL = '" + sedol + "'";
            }
            else if (!string.IsNullOrEmpty(partOfInstrumentName))
            {
                whereClause = "INSTRUMENT_SHORT_NAME LIKE '" + partOfInstrumentName + "%'";
            }
            return Query(@"
SELECT TRADABLE_ENT_ID ID, T.LOCATION_CD LOCATION, INSTRUMENT_SHORT_NAME NAME,
INSTRUMENT_LONG_NAME FULLNAME, FMRCUSIP, SEDOL
FROM INSTRUMENT I 
JOIN TRADABLE_ENT T
ON T.INSTRUMENT_ID = I.INSTRUMENT_ID WHERE ROWNUM <= 100 AND " + whereClause);
        }

        public bool Add(OverrideType type, long id, string code, long pId, long pmId)
        {
            switch (type)
            {
                case OverrideType.ALL:
                    {
                        var rowCount = Execute(@"INSERT INTO IMAP_COMMON.all_country_override 
(all_cntry_override_id, tradable_ent_id, location_cd) 
VALUES (IMAP_COMMON.seq_portfolio_cntry_ovrd_id.nextval, {0}, '{1}')", id, code);
                        return rowCount > 0;
                    }
                case OverrideType.PM:
                    {
                        var rowCount = Execute(@"INSERT INTO IMAP_COMMON.pm_country_override 
(pm_cntry_override_id, tradable_ent_id, portfolio_manager_id, location_cd) 
VALUES (IMAP_COMMON.seq_pm_cntry_override_id.nextval, {0}, {1}, '{2}')", id, pmId, code);
                        return rowCount > 0;
                    }
                case OverrideType.PORTFOLIO:
                    {
                        var rowCount = Execute(@"INSERT INTO IMAP_COMMON.portfolio_country_override 
(portfolio_cntry_override_id, tradable_ent_id, portfolio_id, location_cd) 
VALUES (IMAP_COMMON.seq_portfolio_cntry_ovrd_id.nextval, {0}, {1}, '{2}')", id, pId, code);
                        return rowCount > 0;
                    }
            }
            throw new InvalidOverrideTypeException(type);
        }

        public bool Edit(OverrideType type, long id, string countryCode,
            long portfolioId, long portfolioManagerId,
            long oldPortfolioId, long oldPortfolioManagerId)
        {
            switch (type)
            {
                case OverrideType.ALL:
                    {
                        var rowCount = Execute(@"UPDATE IMAP_COMMON.all_country_override 
SET location_cd = '{0}' WHERE tradable_ent_id = {1}", countryCode, id);
                        return rowCount > 0;
                    }
                case OverrideType.PM:
                    {
                        var rowCount = Execute(@"UPDATE IMAP_COMMON.pm_country_override 
SET location_cd = '{0}', portfolio_manager_id = {1} 
WHERE tradable_ent_id = {2} AND portfolio_manager_id = {3}", countryCode, portfolioManagerId, id, oldPortfolioManagerId);
                        return rowCount > 0;
                    }
                case OverrideType.PORTFOLIO:
                    {
                        var rowCount = Execute(@"UPDATE IMAP_COMMON.portfolio_country_override 
SET location_cd = '{0}', portfolio_id = {1} 
WHERE tradable_ent_id = {2} AND portfolio_id = {3}", countryCode, portfolioId, id, oldPortfolioId);
                        return rowCount > 0;
                    }
            }
            throw new InvalidOverrideTypeException(type);
        }

        public bool InsertCustomCountry(CustomCountry customCountry)
        {
            var rowCount = Execute(@"
INSERT INTO IMAP_COMMON.LOCATION_EXT
(LOCATION_TYPE_CD,
FMR_LOCATION_CD,
LOCATION_CD,
LOCATION_DESC,
DATA_OWNER_CD,
EFFECTIVE_DATE,
DEACTIVATION_DATE,
CREATED_DATETIME,
CREATED_USER,
UPDATED_DATETIME,
UPDATED_USER)
VALUES
('COUNTRY','{0}','{1}','{2}','MAPS',NULL,NULL,SYSDATE,'{3}',SYSDATE,'{3}')
", customCountry.FmrCode, customCountry.Code, customCountry.Name, Constants.UserName);
            return rowCount == 1;
        }

        public bool UpdateCustomCountry(CustomCountry customCountry)
        {
            var rowCount = Execute(@"
UPDATE IMAP_COMMON.LOCATION_EXT
SET FMR_LOCATION_CD = '{0}',
LOCATION_DESC = '{1}',
UPDATED_DATETIME = SYSDATE,
UPDATED_USER = '{2}'
WHERE
LOCATION_CD = '{3}' AND LOCATION_TYPE_CD = 'COUNTRY'
", customCountry.FmrCode, customCountry.Name, Constants.UserName, customCountry.Code);
            return rowCount == 1;
        }
    }
}