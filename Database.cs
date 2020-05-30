﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Server
{
    public class Database
    {
        private MySqlConnection connection;

        public Database()
        {
            this.connection = new MySqlConnection("server=localhost;database=server;uid=root;pwd=;");
        }

        public void ExecuteSql(string pReq)
        {
            this.connection.Open();

            MySqlCommand cmd = new MySqlCommand(pReq, this.connection);
            cmd.ExecuteNonQuery();

            this.connection.Close();
        }

        public Dictionary<int, List<string>> SelectData(string pReq, int pnColumns)
        {
            this.connection.Open();

            Dictionary<int, List<string>> data = new Dictionary<int, List<string>>();

            MySqlCommand cmd = new MySqlCommand(pReq, this.connection);
            MySqlDataReader reader = cmd.ExecuteReader();

            int id = 1;

            List<string> response;

            while (reader.Read())
            {
                response = new List<string>();
                for(int i=0; i < pnColumns; i++)
                {
                    response.Add(reader.GetString(i));
                }
                data.Add(id, response);
                id++;
            }

            this.connection.Close();

            return data;
        }
    }
}
