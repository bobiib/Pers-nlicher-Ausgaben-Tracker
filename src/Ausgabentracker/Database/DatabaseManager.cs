using Microsoft.Data.Sqlite;
using System;
using System.IO;

namespace Ausgabentracker.Database
{
    public class DatabaseManager
    {
        private static DatabaseManager _instance;
        private string _dbFilePath = "Ausgaben.db";
        private string _connectionString;

        private DatabaseManager()
        {
            _connectionString = $"Data Source={_dbFilePath}";
        }

        public static DatabaseManager Instance
        {
            get
            {
                if (_instance == null) _instance = new DatabaseManager();
                return _instance;
            }
        }

        public void InitializeDatabase()
        {
            bool dbExists = File.Exists(_dbFilePath);

            if (!dbExists)
            {
                Console.WriteLine("Datenbank wird erstellt...");

                using (var connection = new SqliteConnection(_connectionString))
                {
                    connection.Open();
                }
                ExecuteScript(@"..\..\..\..\SQL_Scripts\01_CreateDB.sql");
                ExecuteScript(@"..\..\..\..\..\SQL_Scripts\01_CreateDB.sql");

                ExecuteScript(@"..\..\..\..\SQL_Scripts\03_SeedData.sql");
                ExecuteScript(@"..\..\..\..\..\SQL_Scripts\03_SeedData.sql");
            }

            try
            {
                ExecuteScript(@"..\..\..\..\SQL_Scripts\02_UpdateDB.sql");
                ExecuteScript(@"..\..\..\..\..\SQL_Scripts\02_UpdateDB.sql");
            }
            catch (Exception) { }
        }

        private void ExecuteScript(string filePath)
        {
            if (File.Exists(filePath))
            {
                string script = File.ReadAllText(filePath);
                using (var connection = new SqliteConnection(_connectionString))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = script;
                    command.ExecuteNonQuery();
                }
            }
        }

        public int ExecuteNonQuery(string sql)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = sql;
                return command.ExecuteNonQuery();
            }
        }
    }
}