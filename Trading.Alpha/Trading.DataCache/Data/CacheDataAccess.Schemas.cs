namespace Trading.DataCache.Data
{
    public partial class CacheDataAccess
    {
        public void CreateSecurityTable()
        {
            if (!CheckSchemaExists("TRADING"))
            {
                CreateSchema("TRADING");
            }
            if (CheckTableExists("TRADING", "SECURITIES"))
            {
                //      DropTable("SECURITIES");
            }
            CreateTable(
@"CREATE TABLE IF NOT EXISTS SECURITIES (
	ID INT NOT NULL AUTO_INCREMENT,
	CODE VARCHAR(255) NOT NULL,
	NAME VARCHAR(255) NOT NULL,
    CREATE_TIME DATETIME DEFAULT CURRENT_TIMESTAMP,
    UPDATE_TIME DATETIME ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (`ID`),
    INDEX CODE (`CODE`)
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

        public void CreateSecurityPriceTable()
        {
            if (!CheckSchemaExists("TRADING"))
            {
                CreateSchema("TRADING");
            }
            if (CheckTableExists("TRADING", "PRICES"))
            {
                //DropTable("PRICES");
            }
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
    }
}