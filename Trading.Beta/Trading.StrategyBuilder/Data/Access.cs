﻿using System.Collections.Generic;
using System.Data;
using System.Linq;
using Trading.Common.Data;
using Trading.Common.Entities;
using Trading.Common.Utils;

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
            return table.AsEnumerable().Select(SecurityConvert).OrderBy(s => s.Code).ToList();
        }

        private Security SecurityConvert(DataRow r)
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