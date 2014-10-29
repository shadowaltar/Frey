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

        public Dictionary<long, double[]> GetTwoDaysTopVolumes(DateTime date)
        {
            var results = new Dictionary<long, double[]>();
            MySqlCommand cmd;
            if (!preparedCommands.TryGetValue("GetOneDayTopVolumes", out cmd))
            {
                cmd = new MySqlCommand("SELECT SECID, CLOSE, VOLUME FROM PRICES P WHERE TIME = @tt AND VOLUME > @Vol AND CLOSE > @minClose");
                cmd.Parameters.Add("@tt", MySqlDbType.DateTime);
                cmd.Parameters.Add("@Vol", MySqlDbType.Int32);
                cmd.Parameters.Add("@minClose", MySqlDbType.Double);
                cmd.CommandType = CommandType.Text;
                cmd.Connection = database;
                cmd.Prepare();
                preparedCommands["GetOneDayTopVolumes"] = cmd;
            }
            cmd.Parameters["@tt"].Value = date;
            cmd.Parameters["@Vol"].Value = 200000;
            cmd.Parameters["@minClose"].Value = 20;

            var dt = new DataTable();
            using (var da = new MySqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }
            foreach (var r in dt.AsEnumerable())
            {
                var array = new double[2];
                results[r[0].ConvertLong()] = array;
                array[0] = r[1].ConvertDouble();
                array[1] = r[2].ConvertDouble();
            }
            return results;
        }
    }
}