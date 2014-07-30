using log4net;
using Maintenance.Common.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maintenance.Common.Data
{
    public class FmsDataAccessFactory<T> : IDataAccessFactory<T> where T : FmsDataAccess, new()
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const string ServiceAccountPassword = "sRiyXHbjM+1u0YZG0ZIeStvkvMNWyOyIb+zfT4TUbE8=";

        private readonly object authSyncRoot = new object();
        private string password = string.Empty;

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

            var access = new T();
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
                = @"Data Source = {0},7000;Database = cloning;Application Name = FMStation;
User ID = {1};Password = {2};min pool size=3;ClientHostName={3};Connect Timeout=10";

            string server;
            if (Environments.TryGetValue(Environment, out server))
            {
                return string.Format(connectionString, server, Constants.UserName, password, System.Environment.MachineName);
            }
            throw new InvalidOperationException("Cannot find the server name for environment string: " + Environment);
        }

        protected string CreateLoginConnectionString()
        {
            const string connectionString
                = @"Data Source = {0},7000;Database = cloning;Application Name = FMStation;
User ID = fmstn_user;Password = {1};min pool size=3;ClientHostName={2};Connect Timeout=10";

            var pwd = new AesEncryption().DecryptString(ServiceAccountPassword, "MaintenancePortal");

            string server;
            if (Environments.TryGetValue(Environment, out server))
            {
                return string.Format(connectionString, server, pwd, System.Environment.MachineName);
            }
            throw new InvalidOperationException("Cannot find the server name for environment string: " + Environment);
        }

        /// <summary>
        /// Login the designated FMS database environment.
        /// </summary>
        public void Login()
        {
            try
            {
                var fmstationAccess = new FmsDataAccess();
                fmstationAccess.Connect(CreateLoginConnectionString());

                var dt = fmstationAccess.GetAuthenticateInfo();

                if (dt.Rows.Count > 0)
                {
                    var pw = dt.Rows[0]["Password"].ConvertString().Trim();
                    const int encryptCode = 5;
                    Byte[] asciiVal = Encoding.Default.GetBytes(pw);

                    for (int ctr = 0; ctr < pw.Length; ctr++)
                    {
                        pw = pw.Remove(ctr, 1)
                            .Insert(ctr, Convert.ToChar(asciiVal[ctr] ^ encryptCode).ToString());
                    }
                    password = pw;
                    CanLogin = true;
                }
                else
                {
                    CanLogin = false;
                }
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