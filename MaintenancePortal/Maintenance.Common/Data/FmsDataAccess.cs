using System.Reflection;
using log4net;
using Maintenance.Common.Utils;
using Sybase.Data.AseClient;
using System;
using System.Data;

namespace Maintenance.Common.Data
{
    public class FmsDataAccess : DataAccess, IDisposable
    {
        protected static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private AseConnection database;
        private AseTransaction transaction;

        public void Connect(string connectionString)
        {
            database = new AseConnection(connectionString);

            using (new ReportTime("Database Connection Used: {0}"))
            {
                database.Open();
            }
        }

        public DataTable Query(string query, params object[] args)
        {
            var result = new DataTable();
            using (ReportTime.ReportSql(true, GetSqlCount(), query))
            using (var cmd = new AseCommand())
            {
                cmd.CommandText = string.Format(query, args);
                cmd.Connection = database;
                var reader = cmd.ExecuteReader();

                result.Load(reader);
            }
            return result;
        }

        public int Execute(string query)
        {
            using (new ReportTime("Write used {0}"))
            using (var cmd = new AseCommand())
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

        public DataTable GetAuthenticateInfo()
        {
            var result = new DataTable();
            using (new ReportTime("Read used {0}"))
            using (var cmd = new AseCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spCheckNTUser";
                cmd.Parameters.AddWithValue("@ChkCode", Environment.UserName.ToUpper());
                cmd.Connection = database;
                var reader = cmd.ExecuteReader();

                result.Load(reader);
            }
            return result;
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

        public static bool IsAuthenticated { get; set; }

        public static bool CanLogin { get; set; }
    }
}