using Trading.Common.Utils;

namespace Trading.Common.Data
{
    public partial class TradingDataAccess
    {
        public bool CheckSchemaExists(string schemaName)
        {
            var dt = Query("SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = '{0}'", schemaName);
            return !string.IsNullOrEmpty(dt.FirstOrDefaultValue<string>());
        }

        public bool CheckTableExists(string schemaName, string tableName)
        {
            if (!CheckSchemaExists(schemaName))
            {
                return false;
            }

            var dt = Query("SHOW TABLES LIKE '{0}'", tableName);
            return !string.IsNullOrEmpty(dt.FirstOrDefaultValue<string>());
        }

        public void UseSchema(string schemaName)
        {
            schemaName = schemaName.ToUpperInvariant();
            Execute("USE " + schemaName);
        }

        public bool CreateSchema(string schemaName)
        {
            schemaName = schemaName.ToUpperInvariant();
            var rc = Execute("CREATE SCHEMA {0}", schemaName);
            return rc == 1;
        }

        public bool CreateTable(string tableDefinition)
        {
            var rc = Execute(tableDefinition);
            return rc == 1;
        }

        public bool DropTable(string tableName)
        {
            var rc = Execute("DROP TABLE " + tableName);
            return rc == 1;
        }

        public void Clear(string tableName)
        {
            Execute("TRUNCATE TABLE " + tableName);
            Execute("ALTER TABLE {0} AUTO_INCREMENT = 1", tableName);
        }
    }
}