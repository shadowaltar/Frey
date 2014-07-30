using log4net;
using System;
using System.Collections.Generic;

namespace Maintenance.Common.Data
{
    public class ImapDataAccessFactory<T> : IDataAccessFactory<T> where T : ImapDataAccess, new()
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string Environment { get; set; }

        private readonly Dictionary<string, string> environments = new Dictionary<string, string>();
        public Dictionary<string, string> Environments { get { return environments; } }

        public T New()
        {
            var access = new T();
            var connectionString = Environments[Environment];
            try
            {
                access.Connect(connectionString);
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

        /// <summary>
        /// This method doesn't have any actual usage; IMAP doesn't require additional login process.
        /// </summary>
        public void Login()
        {
            // placeholder method. Additional login for IMAP is not needed.
        }
    }
}