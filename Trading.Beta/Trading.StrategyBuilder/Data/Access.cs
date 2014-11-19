using System.Collections.Generic;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;
using Trading.Common.Data;
using Trading.Common.Entities;
using Trading.Common.Utils;
using Trading.StrategyBuilder.Core;

namespace Trading.StrategyBuilder.Data
{
    public class Access : TradingDataAccess
    {
        public List<Security> FindSecurity(string partialSecurityInfo)
        {
            if (string.IsNullOrWhiteSpace(partialSecurityInfo) || partialSecurityInfo.Trim().Length < -3)
                return null;
            partialSecurityInfo = partialSecurityInfo.ToUpperInvariant();

            var table = Query("SELECT * FROM SECURITIES WHERE CODE LIKE '%{0}%' OR UPPER(NAME) LIKE '%{0}%'", partialSecurityInfo);
            return table.To(SecurityConvert).OrderBy(s => s.Code).ToList();
        }

        public DataTable GetPricesByYear(DataCriteria dataCriteria)
        {
            var result = new DataTable();
            using (var cmd = new MySqlCommand { CommandTimeout = 20000, Connection = database })
            {
                var whereClauses = dataCriteria.ToWhereClauseSplitByYears();
                for (int i = 0; i < whereClauses.Length; i++)
                {
                    var clause = whereClauses[i];
                    cmd.CommandText =
                        "SELECT SECID, TIME, OPEN, HIGH, LOW, CLOSE, ADJCLOSE, VOLUME FROM PRICES P WHERE "
                        + clause;

                    using (var table = new DataTable())
                    {
                        using (ReportTime.Start(" used time: {0}"))
                        using (var da = new MySqlDataAdapter(cmd))
                        {
                            da.Fill(table);
                        }
                        result.Merge(table);
                    }
                }
            }
            return result;
        }
    }
}