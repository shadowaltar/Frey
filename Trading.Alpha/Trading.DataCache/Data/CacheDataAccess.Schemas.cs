using System;
using Trading.Common.Data;

namespace Trading.DataCache.Data
{
    public partial class CacheDataAccess
    {
        public void CreateSecurityTable()
        {
            Execute("DROP TABLE IF EXISTS SECURITIES");
            Execute(
@"CREATE TABLE SECURITIES (
	ID INTEGER PRIMARY KEY,
	CODE VARCHAR(100) NOT NULL,
	NAME VARCHAR(200) NOT NULL
)"
);
        }

        public void CreateSecurityPriceTable()
        {
            Execute("DROP TABLE IF EXISTS PRICES");
            Execute(
@"CREATE TABLE PRICES (
	ID INTEGER PRIMARY KEY,
	SECID INT NOT NULL,
	TIME DATETIME NOT NULL,
	OPEN DOUBLE,
	HIGH DOUBLE,
	LOW  DOUBLE,
	CLOSE DOUBLE,
	VOLUME DOUBLE,
	ADJCLOSE DOUBLE
)"
);
        }
    }
}