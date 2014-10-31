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

        public Dictionary<DateTime, Dictionary<long, Price>> GetOneYearPriceData(int year)
        {
            var start = new DateTime(year, 1, 1);
            var end = new DateTime(year, 12, 31);
            var results = new Dictionary<DateTime, Dictionary<long, Price>>();
            MySqlCommand cmd;
            if (!preparedCommands.TryGetValue("GetOneDayTopVolumes", out cmd))
            {
                cmd = new MySqlCommand("SELECT SECID, TIME, OPEN, HIGH, LOW, CLOSE, VOLUME FROM PRICES P WHERE TIME >= @TStart AND TIME <= @TEnd");
                cmd.Parameters.Add("@TStart", MySqlDbType.DateTime);
                cmd.Parameters.Add("@TEnd", MySqlDbType.DateTime);
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
            foreach (var r in dt.AsEnumerable())
            {
                var id = r[0].ConvertLong();
                var time = r["TIME"].ConvertDate();
                Dictionary<long, Price> ps;
                if (!results.TryGetValue(time, out ps))
                {
                    results[time] = new Dictionary<long, Price>();
                }
                ps = results[time];
                var p = new Price
                {
                    At = r["TIME"].ConvertDate(),
                    Open = r["OPEN"].ConvertDecimal(),
                    High = r["HIGH"].ConvertDecimal(),
                    Low = r["LOW"].ConvertDecimal(),
                    Close = r["CLOSE"].ConvertDecimal(),
                    Volume = r["VOLUME"].ConvertLong(),
                };
                ps[id] = p;
            }
            return results;
        }
    }
}