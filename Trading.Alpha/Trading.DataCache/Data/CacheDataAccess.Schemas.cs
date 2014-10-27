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
    INDEX CODE (`CODE`)
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
	TIME DATETIME NOT NULL,
	OPEN DOUBLE,
	HIGH DOUBLE,
	LOW  DOUBLE,
	CLOSE DOUBLE,
	VOLUME DOUBLE,
	ADJCLOSE DOUBLE,
    PRIMARY KEY (`ID`)
) ENGINE=MyISAM"
);
        }

        public void CreatePriceTableConstraint()
        {
            ExecuteLongRun(@"
CREATE INDEX UQ_PRICE_TIME
ON PRICES (TIME);");
            ExecuteLongRun(@"
CREATE UNIQUE INDEX UQ_PRICE_SEC_TIME
ON PRICES (SECID, TIME);");
        }
    }
}