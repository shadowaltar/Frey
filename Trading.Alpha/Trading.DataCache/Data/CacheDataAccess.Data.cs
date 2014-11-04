﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Trading.Common.Entities;
using Trading.Common.Utils;

namespace Trading.DataCache.Data
{
    public partial class CacheDataAccess
    {
        public void AddSecurity(string code, string name)
        {
            Execute(@"INSERT INTO SECURITIES (CODE,NAME) VALUES ('{0}','{1}')", code, name);
        }

        public void AddPrice(long securityId, DateTime time, double open, double high, double low, double close, double volume, double adjClose)
        {
            var sb = new StringBuilder();
            sb.Append("INSERT INTO PRICES (SECID,TIME,OPEN,HIGH,LOW,CLOSE,VOLUME,ADJCLOSE) VALUES (")
                .Append(securityId).Append(',')
                .Append(time.Year * 10000 + time.Month * 100 + time.Day).Append(',')
                .Append(open).Append(',')
                .Append(high).Append(',')
                .Append(low).Append(',')
                .Append(close).Append(',')
                .Append(volume).Append(',')
                .Append(adjClose).Append(')');
            Execute(sb.ToString());
        }

        public Security GetSecurity(string code)
        {
            return Query("SELECT * FROM SECURITIES WHERE CODE = '{0}'", code).FirstOrDefault(SecurityConvert);
        }

        public List<Security> GetSecurities()
        {
            return Query("SELECT * FROM SECURITIES").To(SecurityConvert).ToList();
        }

        private Security SecurityConvert(DataRow r)
        {
            return new Security
            {
                Id = r["ID"].ConvertLong(),
                Code = r["CODE"].ConvertString(),
                Name = r["NAME"].ConvertString(),
            };
        }
    }
}