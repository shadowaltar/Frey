namespace Trading.DataCache.Data
{
    public partial class CacheDataAccess
    {
        public void AddSecurity(string code, string name)
        {
            Execute(@"INSERT INTO SECURITIES (CODE,NAME) VALUES ('{0}','{1}')", code, name);
        }
        public void AddPrice(string securityId, double open, double high, double low, double close, double volume, double adjClose)
        {
          //  Execute(@"INSERT INTO PRICES (CODE,NAME) VALUES ('{0}','{1}')", code, name);
        }
    }
}