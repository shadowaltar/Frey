using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using Trading.Common.Entities;
using Trading.Common.Utils;

namespace Trading.Backtest.Data
{
    public partial class BacktestDataAccess
    {
        protected Dictionary<string, MySqlCommand> preparedCommands = new Dictionary<string, MySqlCommand>();

        public IEnumerable<Security> GetAllSecurities()
        {
            MySqlCommand cmd = null;
            MySqlDataReader reader = null;
            try
            {
                cmd = new MySqlCommand
                {
                    CommandText = "SELECT ID, CODE, NAME FROM SECURITIES",
                    Connection = database
                };
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var s = new Security
                    {
                        Id = reader["ID"].ConvertInt(),
                        Code = reader["Code"].ToString(),
                        Name = reader["NAME"].ToString()
                    };
                    yield return s;
                }
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();
                if (reader != null)
                    reader.Dispose();
            }
        }

        public IEnumerable<Price> GetOneYearPriceData(int year)
        {
            var start = new DateTime(year, 1, 1).ToDateInt();
            var end = new DateTime(year, 12, 31).ToDateInt();
            MySqlCommand cmd;
            if (!preparedCommands.TryGetValue("GetOneDayTopVolumes", out cmd))
            {
                cmd = new MySqlCommand("SELECT SECID, TIME, HIGH, LOW, CLOSE, ADJCLOSE, VOLUME FROM PRICES P WHERE TIME >= @TStart AND TIME <= @TEnd AND VOLUME > 0");
                cmd.Parameters.Add("@TStart", MySqlDbType.Int32);
                cmd.Parameters.Add("@TEnd", MySqlDbType.Int32);
                cmd.CommandType = CommandType.Text;
                cmd.Connection = database;
                cmd.CommandTimeout = 30000;
                cmd.Prepare();
                preparedCommands["GetOneDayTopVolumes"] = cmd;
            }
            cmd.Parameters["@TStart"].Value = start;
            cmd.Parameters["@TEnd"].Value = end;

            var dt = new DataTable();
            using (var da = new MySqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }
            return dt.To(r => new Price
            {
                SecId = r[0].ConvertLong(),
                At = r["TIME"].ConvertDate("yyyyMMdd"),
                Open = double.NaN,
                High = r["HIGH"].ConvertDouble(),
                Low = r["LOW"].ConvertDouble(),
                Close = r["CLOSE"].ConvertDouble(),
                AdjClose = r["ADJCLOSE"].ConvertDouble(),
                Volume = r["VOLUME"].ConvertLong(),
            });
        }
    }
}