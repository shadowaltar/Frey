using log4net;
using Maintenance.Common.Utils;
using Oracle.DataAccess.Client;
using System;
using System.Data;
using System.Reflection;

namespace Maintenance.Common.Data
{
    /// <summary>
    /// Class to facilitate database read/write operations. The class must be
    /// initialized by <see cref="IDataAccessFactory{T}.New{T}"/> or
    /// by <see cref="IDataAccessFactory{T}.NewTransaction{T}"/>, by providing the
    /// specific implementation in their generic type parameter.
    /// </summary>
    public partial class ImapDataAccess : DataAccess, IDisposable
    {
        protected static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        private OracleConnection database;
        private OracleTransaction transaction;

        public void Connect(string connectionString)
        {
            database = new OracleConnection(connectionString);

            using (new ReportTime("Database Connection Used: {0}"))
            {
                database.Open();
            }
        }

        /// <summary>
        /// Execute a SQL and 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public DataTable Query(string query, params object[] args)
        {
            var result = new DataTable();

            if (args != null && args.Length > 0)
            {
                query = string.Format(query, args);
            }

            using (ReportTime.ReportSql(true, GetSqlCount(), query))
            using (var cmd = new OracleCommand(query, database))
            using (var reader = cmd.ExecuteReader())
            {
                result.Load(reader);
            }

            return result;
        }

        /// <summary>
        /// Execute a SQL and return the count of rows affected.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public int Execute(string query, params object[] args)
        {
            if (args != null && args.Length > 0)
            {
                query = string.Format(query, args);
            }

            using (ReportTime.ReportSql(false, GetSqlCount(), query))
            using (var cmd = new OracleCommand(query, database))
            {
                cmd.CommandTimeout = 30;
                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Close the data access instance. If it is a transaction created by 
        /// <see cref="ImapDataAccessFactory{T}.NewTransaction{T}"/>, the transaction
        /// will be committed.
        /// </summary>
        public void Dispose()
        {
            if (transaction != null)
            {
                Log.InfoFormat("The transaction ({0}) is committed.", currentTransactionNumber.ToString("d5"));
                transaction.Commit();
            }
            if (database != null)
            {
                database.Close();
            }
        }

        /// <summary>
        /// Open a transaction. This should usually only be used by the 
        /// <see cref="ImapDataAccessFactory{T}.NewTransaction"/>.
        /// </summary>
        public override void OpenTransaction()
        {
            transaction = database.BeginTransaction();
            currentTransactionNumber = GetTransactionCount();
            Log.InfoFormat("The transaction ({0}) is opened.", currentTransactionNumber.ToString("d5"));
        }

        /// <summary>
        /// Rollback a transaction. This should usually only be used by a <see cref="ImapDataAccess"/>
        ///  created by <see cref="ImapDataAccessFactory{T}.NewTransaction"/>
        /// If this is called by a non-transaction <see cref="ImapDataAccess"/>, it will not have any effect.
        /// </summary>
        public override void Rollback()
        {
            if (transaction != null)
            {
                Log.InfoFormat("The transaction ({0}) is rolled back.", currentTransactionNumber.ToString("d5"));
                transaction.Rollback();
                transaction = null;
            }
        }
    }
}