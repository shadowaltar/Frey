using log4net;
using Trading.Common.Data;
using System;
using System.Collections.Generic;

namespace Trading.DataCache.Data
{
    public class CacheDataAccessFactory<T> : IDataAccessFactory<T> where T : CacheDataAccess, new()
    {
        public CacheDataAccessFactory(IDataCache cache)
        {
            this.cache = cache;
        }

        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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

            var access = new T { DataCache = cache };
            try
            {
                access.Connect();
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
            try
            {
                access.Connect();
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