using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using Trading.Common.Entities;
using Trading.Common.Utils;
using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace Trading.Common.Data
{
    public partial class TradingDataAccess : DataAccess, IDisposable
    {
        protected static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public IDataCache DataCache { get; set; }

        private bool isRollback;

        private MySqlConnection database;
        private MySqlTransaction transaction;

        public void Connect(string connectionString)
        {
            if (isRollback)
                throw new InvalidOperationException("Cannot connect when the transaction rollback is done.");

            database = new MySqlConnection(connectionString);

            using (new ReportTime("Database Connection Used: {0}"))
            {
                database.Open();
            }
        }

        public DataTable Query(string query, params object[] args)
        {
            var result = new DataTable();
            using (new ReportTime())
            using (var cmd = new MySqlCommand())
            {
                cmd.CommandText = string.Format(query, args);
                cmd.Connection = database;
                var reader = cmd.ExecuteReader();

                result.Load(reader);
            }
            return result;
        }

        public IEnumerable<T> YieldQuery<T>(Func<IDataReader, T> converter, string query, params object[] args)
        {
            using (ReportTime.ReportSql(true, GetSqlCount(), query))
            using (var cmd = new MySqlCommand())
            {
                cmd.CommandText = string.Format(query, args);
                cmd.Connection = database;
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    yield return converter(reader);
                }
            }
        }

        public int Execute(string query)
        {
            if (isRollback)
                throw new InvalidOperationException("Cannot execute when the transaction rollback is done.");

            using (new ReportTime("Write used {0}"))
            using (var cmd = new MySqlCommand())
            {
                cmd.CommandText = query;
                cmd.Connection = database;
                return cmd.ExecuteNonQuery();
            }
        }

        public int Execute(string query, params object[] args)
        {
            if (isRollback)
                throw new InvalidOperationException("Cannot execute when the transaction rollback is done.");

            using (var cmd = new MySqlCommand())
            {
                cmd.CommandText = string.Format(query, args);
                cmd.Connection = database;
                return cmd.ExecuteNonQuery();
            }
        }

        public override void OpenTransaction()
        {
            if (isRollback)
                throw new InvalidOperationException("Cannot open another transaction when the transaction rollback is done.");

            transaction = database.BeginTransaction();
            currentTransactionNumber = GetTransactionCount();
            Log.InfoFormat("The transaction ({0}) is opened.", currentTransactionNumber.ToString("d5"));
        }

        public override void Commit()
        {
            if (transaction != null && !isRollback)
            {
                transaction.Commit();
            }
        }

        public override void Rollback()
        {
            if (transaction != null)
            {
                if (!isRollback)
                {
                    Log.InfoFormat("The transaction ({0}) is rolled back.", currentTransactionNumber.ToString("d5"));
                    transaction.Rollback();
                }
                transaction = null;
                isRollback = true;
            }
        }

        public void Dispose()
        {
            if (transaction != null && !isRollback)
            {
                Log.InfoFormat("The transaction ({0}) is committed.", currentTransactionNumber.ToString("d5"));
                transaction.Commit();
            }
            if (database != null)
            {
                database.Close();
                Console.WriteLine("DB Conn Closed");
            }
        }
    }
}