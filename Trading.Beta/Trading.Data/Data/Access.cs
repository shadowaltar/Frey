using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using Trading.Common.Data;
using Trading.Common.Entities;
using Trading.Common.Utils;

namespace Trading.Data.Data
{
    public class Access : TradingDataAccess
    {
        public void AddSecurity(string code, string name)
        {
            Execute(@"INSERT INTO SECURITIES (CODE,NAME) VALUES ('{0}','{1}')", code, name);
        }

        public void AddCalendar(string country, int date, int holiday, int mktClose)
        {
            Execute(@"INSERT INTO CALENDAR (COUNTRY,DATE,HOLIDAY,MARKET_CLOSED) VALUES ('{0}',{1},{2},{3})",
                country, date, holiday, mktClose);
        }

        public void AddPrice(long securityId, int time, double open, double high, double low, double close, double volume, double adjClose)
        {
            var sb = new StringBuilder();
            sb.Append("INSERT INTO PRICES (SECID,TIME,OPEN,HIGH,LOW,CLOSE,VOLUME,ADJCLOSE) VALUES (")
                .Append(securityId).Append(',')
                .Append(time).Append(',')
                .Append(open).Append(',')
                .Append(high).Append(',')
                .Append(low).Append(',')
                .Append(close).Append(',')
                .Append(volume).Append(',')
                .Append(adjClose).Append(')');
            Execute(sb.ToString());
        }

        public void AddCountry(string code, string name)
        {
            Execute(@"INSERT INTO COUNTRIES (CODE,NAME) VALUES ('{0}','{1}')", code, name);
        }

        public Security GetSecurity(string code)
        {
            return Query("SELECT * FROM SECURITIES WHERE CODE = '{0}'", code).FirstOrDefault(SecurityConvert);
        }

        public List<Security> GetSecurities()
        {
            return Query("SELECT * FROM SECURITIES").To(SecurityConvert).ToList();
        }

        public void CreateSecurityTable()
        {
            if (!CheckSchemaExists("TRADING"))
            {
                CreateSchema("TRADING");
            }
            if (CheckTableExists("TRADING", "SECURITIES"))
            {
                DropTable("SECURITIES");
            }
            CreateTable(
@"CREATE TABLE IF NOT EXISTS SECURITIES (
	ID INT NOT NULL AUTO_INCREMENT,
	CODE VARCHAR(255) NOT NULL,
	NAME VARCHAR(255) NOT NULL,
    CREATE_TIME DATETIME DEFAULT CURRENT_TIMESTAMP,
    UPDATE_TIME DATETIME ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (`ID`),
    INDEX IX_SECURITIES_CODE (`CODE`)
) ENGINE=MyISAM"
);
        }

        public void CreateCalendarTable()
        {
            if (!CheckSchemaExists("TRADING"))
            {
                CreateSchema("TRADING");
            }
            if (CheckTableExists("TRADING", "CALENDAR"))
            {
                DropTable("CALENDAR");
            }
            CreateTable(
@"CREATE TABLE IF NOT EXISTS CALENDAR (
	ID INT NOT NULL AUTO_INCREMENT,
	COUNTRY CHAR(3) NOT NULL,
    DATE INT NOT NULL,
	HOLIDAY BOOL DEFAULT 0,
    MARKET_CLOSED BOOL DEFAULT 0,
    CREATE_TIME DATETIME DEFAULT CURRENT_TIMESTAMP,
    UPDATE_TIME DATETIME ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (`ID`,`COUNTRY`)
) ENGINE=MyISAM"
);
        }

        public void CreateCountryTable()
        {
            if (!CheckSchemaExists("TRADING"))
            {
                CreateSchema("TRADING");
            }
            if (CheckTableExists("TRADING", "COUNTRIES"))
            {
                DropTable("COUNTRIES");
            }
            CreateTable(
@"CREATE TABLE IF NOT EXISTS COUNTRIES (
	ID INT NOT NULL AUTO_INCREMENT,
	CODE CHAR(3) NOT NULL,
    NAME VARCHAR(50) NOT NULL,
    PRIMARY KEY (`ID`,`CODE`),    
    INDEX IX_COUNTRIES_CODE (`CODE`)
) ENGINE=MyISAM"
);
        }

        public void CreateSecurityPriceTable()
        {
            if (!CheckSchemaExists("TRADING"))
            {
                CreateSchema("TRADING");
            }
            if (CheckTableExists("TRADING", "PRICES"))
                DropTable("PRICES");
            CreateTable(
@"CREATE TABLE IF NOT EXISTS PRICES (
	ID INT NOT NULL AUTO_INCREMENT,
	SECID INT NOT NULL,
	TIME INT NOT NULL,
	OPEN DOUBLE,
	HIGH DOUBLE,
	LOW  DOUBLE,
	CLOSE DOUBLE,
	VOLUME DOUBLE,
	ADJCLOSE DOUBLE,
    PRIMARY KEY (`ID`, `TIME`)
) ENGINE=MyISAM
PARTITION BY RANGE COLUMNS (TIME)  (
    PARTITION p1980 VALUES LESS THAN (19800101),
    PARTITION p1990 VALUES LESS THAN (19900101),
    PARTITION p2000 VALUES LESS THAN (20000101),
    PARTITION p2001 VALUES LESS THAN (20010101),
    PARTITION p2002 VALUES LESS THAN (20020101),
    PARTITION p2003 VALUES LESS THAN (20030101),
    PARTITION p2004 VALUES LESS THAN (20040101),
    PARTITION p2005 VALUES LESS THAN (20050101),
    PARTITION p2006 VALUES LESS THAN (20060101),
    PARTITION p2007 VALUES LESS THAN (20070101),
    PARTITION p2008 VALUES LESS THAN (20080101),
    PARTITION p2009 VALUES LESS THAN (20090101),
    PARTITION p2010 VALUES LESS THAN (20100101),
    PARTITION p2011 VALUES LESS THAN (20110101),
    PARTITION p2012 VALUES LESS THAN (20120101),
    PARTITION p2013 VALUES LESS THAN (20130101),
    PARTITION p2014 VALUES LESS THAN (20140101),
    PARTITION pMAX VALUES LESS THAN (MAXVALUE)
)"
);
        }

        public void CreatePriceTableConstraint()
        {
            ExecuteLongRun(@"
CREATE INDEX IX_PRICE_TIME
ON PRICES (TIME)");
            ExecuteLongRun(@"
CREATE INDEX IX_PRICE_SEC
ON PRICES (SECID)");
            ExecuteLongRun(@"
CREATE UNIQUE INDEX UQ_PRICE_SEC_TIME
ON PRICES (SECID, TIME)");
        }

        public int ExecuteLongRun(string query)
        {
            if (isRollback)
                throw new InvalidOperationException("Cannot execute when the transaction rollback is done.");

            using (var cmd = new MySqlCommand())
            {
                cmd.CommandText = query;
                cmd.Connection = database;
                cmd.CommandTimeout = 1000000;
                return cmd.ExecuteNonQuery();
            }
        }

        public MySqlCommand GetCommonCommand()
        {
            var cmd = new MySqlCommand { Connection = database };
            return cmd;
        }

        public List<int> GetCalendar(string country)
        {
            var table = Query("SELECT DATE FROM CALENDAR WHERE COUNTRY = '{0}' AND (HOLIDAY > 0 OR MARKET_CLOSED > 0)", country);
            return table.To(r => r["DATE"].Int()).ToList();
        }
    }
}