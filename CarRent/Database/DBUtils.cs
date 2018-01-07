using System;
using MySql.Data.MySqlClient;
using Npgsql;
using System.Data.Common;
using CarRent.Helper;

namespace CarRent.Database
{
    class DBUtils
    {
        private static volatile int connectionId;

        public static MySqlConnection GetMySQLDBConnection()
        {
            string host = "localhost";
            int port = 3306;
            string database = "bdd_car_rent";
            string username = "root";
            string password = "13579";

            String connString = "Server=" + host + ";Database=" + database
                + ";port=" + port + ";User Id=" + username + ";password=" + password;

            MySqlConnection connection = new MySqlConnection(connString);

            return connection;
        }

        public static NpgsqlConnection GetPostgreSQLDBConnection()
        {
            string host = "localhost";
            int port = 5432;
            string database = "bdd_car_rent";
            string username = "postgres";
            string password = "123456";

            String connString = "Server=" + host + ";Database=" + database
                + ";port=" + port + ";User Id=" + username + ";password=" + password;

            NpgsqlConnection connection = new NpgsqlConnection(connString);

            return connection;
        }

        public static DbDataReader ExecuteCommand(string commandString, DbConnection connection)
        {
            try
            {
                string databaseType = connection.GetType() == typeof(MySqlConnection) ? "MySQL" : "PostgreSQL";
                DebugLog.WriteLine("Executing " + databaseType + " command : \n" + commandString);
                connection.Open();
                DbCommand command = connection.CreateCommand();
                command.CommandText = commandString;

                DbDataReader reader = command.ExecuteReader();
                return reader;
            }
            catch (Exception ex)
            {
                DebugLog.WriteLine(ex);
                throw ex;
            }
        }

    }
}