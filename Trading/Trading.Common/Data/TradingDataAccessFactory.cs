using log4net;
using Trading.Common.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Trading.Common.Data
{
    public class TradingDataAccessFactory<T> : IDataAccessFactory<T> where T : TradingDataAccess, new()
    {
        public TradingDataAccessFactory(IDataCache cache)
        {
            this.cache = cache;
        }


        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const string ServiceAccountPassword = "sRiyXHbjM+1u0YZG0ZIeStvkvMNWyOyIb+zfT4TUbE8=";

        private readonly object authSyncRoot = new object();

        public static bool IsAuthenticated { get; set; }
        public static bool CanLogin { get; set; }

        private string environment;
        public string Environment
        {
            get { return environment; }
            set
            {
                if (environment != value)
                {
                    environment = value;
                    IsAuthenticated = false;
                }
            }
        }

        private readonly Dictionary<string, string> environments = new Dictionary<string, string>();
        private IDataCache cache;
        public Dictionary<string, string> Environments { get { return environments; } }

        public T New()
        {
            lock (authSyncRoot)
            {
                if (!IsAuthenticated)
                    Login();
            }

            if (!CanLogin)
                throw new Exception("Cannot login to database.");

            var access = new T {DataCache = cache};
            try
            {
                access.Connect(CreateConnectionString());
            }
            catch (Exception e)
            {
                Log.Error("Cannot connect to the database: " + Environment);
                throw new Exception("Cannot connect to the database: " + Environment, e);
            }
            return access;
        }

        public T NewTransaction()
        {
            var access = new T();
            var connectionString = Environments[Environment];
            try
            {
                access.Connect(connectionString);
                access.OpenTransaction();
            }
            catch (Exception e)
            {
                access.Rollback();
                Log.Info("Cannot connect to the database: " + Environment);
                throw new Exception("Cannot connect to the database: " + Environment, e);
            }
            return access;
        }

        protected string CreateConnectionString()
        {
            const string connectionString
                = @"Server=127.0.0.1;Database=References;Uid=trading;Pwd=trading;ConvertZeroDatetime=True;
UseUsageAdvisor=True;Minimum Pool Size=10;maximum pool size=50;Keepalive=10;Connection Timeout=5;default command timeout=30";
            return connectionString;
        }

        protected string CreateLoginConnectionString()
        {
            const string connectionString
                = @"Data Source = {0},7000;Database = cloning;Application Name = FMStation;
User ID = fmstn_user;Password = {1};min pool size=3;ClientHostName={2};Connect Timeout=10";

            var pwd = new AesEncryption().DecryptString(ServiceAccountPassword, "TradingPortal");

            string server;
            if (Environments.TryGetValue(Environment, out server))
            {
                return string.Format(connectionString, server, pwd, System.Environment.MachineName);
            }
            throw new InvalidOperationException("Cannot find the server name for environment string: " + Environment);
        }

        /// <summary>
        /// Login the designated database environment.
        /// </summary>
        public void Login()
        {
            try
            {
                CanLogin = true;
                IsAuthenticated = true;
            }
            catch (Exception e)
            {
                Log.Error("Failed to login to FMStation database.", e);
                CanLogin = false;
            }
        }
    }
}