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

        private MySqlConnection database;
        private MySqlTransaction transaction;

        public void Connect(string connectionString)
        {
            database = new MySqlConnection(connectionString);

            using (new ReportTime("Database Connection Used: {0}"))
            {
                database.Open();
            }
        }

        public DataTable Query(string query, params object[] args)
        {
            var result = new DataTable();
            using (ReportTime.ReportSql(true, GetSqlCount(), query))
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
            using (new ReportTime("Write used {0}"))
            using (var cmd = new MySqlCommand())
            {
                cmd.CommandText = query;
                cmd.Connection = database;
                return cmd.ExecuteNonQuery();
            }
        }

        public override void OpenTransaction()
        {
            transaction = database.BeginTransaction();
            currentTransactionNumber = GetTransactionCount();
            Log.InfoFormat("The transaction ({0}) is opened.", currentTransactionNumber.ToString("d5"));
        }

        public override void Rollback()
        {
            if (transaction != null)
            {
                Log.InfoFormat("The transaction ({0}) is rolled back.", currentTransactionNumber.ToString("d5"));
                transaction.Rollback();
                transaction = null;
            }
        }

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
                Console.WriteLine("DB Conn Closed");
            }
        }
    }
}