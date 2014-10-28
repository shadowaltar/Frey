using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using Trading.Common.Entities;

namespace Trading.Backtest.Data
{
    public partial class BacktestDataAccess
    {
        public Dictionary<int, Security> GetAllSecurities()
        {
            using (var cmd = new MySqlCommand())
            {
                cmd.CommandText = "SELECT ID, CODE, NAME FROM SECURITIES";
                cmd.Connection = database;
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var id = reader["ID"];
                        Console.WriteLine(reader["NAME"]);
                    }
                }
            }
            return new Dictionary<int, Security>();
        }
    }
}