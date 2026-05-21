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
                ExecuteScript("01_CreateDB.sql");
                ExecuteScript("03_SeedData.sql");
            }

            try
            {
                ExecuteScript("02_UpdateDB.sql");
            }
            catch (Exception) { }
        }

        private void ExecuteScript(string fileName)
        {
            string filePath = FindScriptPath(fileName);

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
                return;
            }

            throw new FileNotFoundException($"SQL-Script wurde nicht gefunden: {fileName}", fileName);
        }

        private string FindScriptPath(string fileName)
        {
            string[] startDirectories =
            {
                AppDomain.CurrentDomain.BaseDirectory,
                Environment.CurrentDirectory,
                Directory.GetCurrentDirectory()
            };

            foreach (string startDirectory in startDirectories)
            {
                var directory = new DirectoryInfo(startDirectory);

                while (directory != null)
                {
                    string scriptPath = Path.Combine(directory.FullName, "SQL_Scripts", fileName);
                    if (File.Exists(scriptPath))
                    {
                        return scriptPath;
                    }

                    directory = directory.Parent;
                }
            }

            return null;
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
