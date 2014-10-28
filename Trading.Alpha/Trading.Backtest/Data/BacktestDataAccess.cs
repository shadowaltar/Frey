using System.Data;
using MySql.Data.MySqlClient;
using Trading.Common.Data;
using System;
using Trading.Common.Utils;

namespace Trading.Backtest.Data
{
    public partial class BacktestDataAccess : TradingDataAccess
    {
        public override void Connect(string connectionString)
        {
            if (isRollback)
                throw new InvalidOperationException("Cannot connect when the transaction rollback is done.");

            database = new MySqlConnection(connectionString);
            database.Open();
        }

        public override int Execute(string query)
        {
            if (isRollback)
                throw new InvalidOperationException("Cannot execute when the transaction rollback is done.");

            using (var cmd = new MySqlCommand())
            {
                cmd.CommandText = query;
                cmd.Connection = database;
                return cmd.ExecuteNonQuery();
            }
        }

        public int ExecuteLongRun(string query)
        {
            if (isRollback)
                throw new InvalidOperationException("Cannot execute when the transaction rollback is done.");

            using (var cmd = new MySqlCommand())
            {
                cmd.CommandText = query;
                cmd.Connection = database;
                cmd.CommandTimeout = 1000000;
                return cmd.ExecuteNonQuery();
            }
        }

        public override int Execute(string query, params object[] args)
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

        public DataTable QueryEx(MySqlCommand cmd, string query, params object[] args)
        {
            var result = new DataTable();
            cmd.CommandText = string.Format(query, args);
            cmd.Connection = database;
            using (var reader = cmd.ExecuteReader())
                result.Load(reader);
            return result;
        }
    }
}