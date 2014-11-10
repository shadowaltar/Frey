using log4net;
using System;
using Trading.Common.Data;

namespace Trading.StrategyBuilder.Data
{
    public class AccessFactory<T> : IDataAccessFactory<T> where T : Access, new()
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected string CreateConnectionString()
        {
            const string connectionString
                = @"Server=127.0.0.1;Database=trading;Uid=trading;Pwd=trading;ConvertZeroDatetime=True;
UseUsageAdvisor=True;Minimum Pool Size=10;maximum pool size=50;Connection Timeout=5;default command timeout=20";
            return connectionString;
        }

        public T New()
        {
            var access = new T();
            access.Connect(CreateConnectionString());
            return access;
        }

        public T NewTransaction()
        {
            var access = new T();
            access.Connect(CreateConnectionString());
            try
            {
                access.OpenTransaction();
            }
            catch (Exception e)
            {
                access.Rollback();
                Log.Info("Cannot connect to the database");
                throw new Exception("Cannot connect to the database", e);
            }
            return access;
        }
    }
}