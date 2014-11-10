using log4net;
using System;

namespace Trading.Common.Data
{
    public class TradingDataAccessFactory<T> : IDataAccessFactory<T> where T : TradingDataAccess, new()
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public T New()
        {
            var access = new T();
            try
            {
                access.Connect(CreateConnectionString());
            }
            catch (Exception e)
            {
                Log.Error("Cannot connect to the database");
                throw new Exception("Cannot connect to the database", e);
            }
            return access;
        }

        public T NewTransaction()
        {
            var access = new T();
            try
            {
                access.Connect(CreateConnectionString());
                access.OpenTransaction();
            }
            catch (Exception e)
            {
                Log.Error("Cannot connect to the database");
                throw new Exception("Cannot connect to the database", e);
            }
            return access;
        }

        protected string CreateConnectionString()
        {
            const string connectionString
                = @"Server=127.0.0.1;Database=References;Uid=trading;Pwd=trading;ConvertZeroDatetime=True;
UseUsageAdvisor=True;Minimum Pool Size=10;maximum pool size=50;Connection Timeout=5;default command timeout=20";
            return connectionString;
        }
    }
}