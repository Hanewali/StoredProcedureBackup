using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace StoredProceduresBackup
{
    public static class Program
    {
        private static List<SqlObject> _storedProcedures;
        private static List<SqlObject> _userDefinedFunctions;
        private static Configuration _configuration;
        private static string _proceduresQuery;
        private static string _functionsQuery;
        private static SqlCommand _proceduresCommand;
        private static ServerConnection _serverConnection;
        private static Server _server;
        private static Database _database;
        private static SqlObjects _sqlObjects;
        private static SqlConnection _connection;
        private static SqlCommand _functionsCommand;

        private static void Prepare()
        {
            _configuration = new Configuration();
            _storedProcedures = new List<SqlObject>();
            _userDefinedFunctions = new List<SqlObject>();
            _proceduresQuery = _configuration.GetProceduresQuery();
            _functionsQuery = _configuration.GetFunctionsQuery();
        }

        private static void PrepareConnection(string connectionString)
        {
            _connection = new SqlConnection(connectionString);
            _connection.Open();
            _proceduresCommand = new SqlCommand(_proceduresQuery, _connection);
            _functionsCommand = new SqlCommand(_functionsQuery, _connection);
            _serverConnection = new ServerConnection(_connection);
            _server = new Server(_serverConnection);
            _database = _server.Databases[_connection.Database];
            _sqlObjects = new SqlObjects(_database.Name);
            _storedProcedures = new List<SqlObject>();
            _userDefinedFunctions = new List<SqlObject>();
        }
        
        static void Main()
        {
            Prepare();

            foreach (var connectionString in _configuration.ConnectionStrings)
            {
                PrepareConnection(connectionString);
                ReadStoredProcedures(_proceduresCommand);
                ReadUserDefinedFunctions(_functionsCommand);
                GetProcedures();
                _sqlObjects.Save(); 
            }
        }

        

        private static void GetProcedures()
        {
            foreach (var procedureObject in _storedProcedures)
            {
                _sqlObjects.Procedures.Add(new StoredProcedure(_database, procedureObject.Name, procedureObject.SchemaName));
            }

            foreach (var functionsObject in _userDefinedFunctions)
            {
                _sqlObjects.Functions.Add(new UserDefinedFunction(_database, functionsObject.Name, functionsObject.SchemaName));
            }
        }
        
        private static void ReadStoredProcedures(SqlCommand command)
        {
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                _storedProcedures.Add(new SqlObject
                (
                    reader["schema"].ToString(),
                    reader["name"].ToString()
                ));
            }
            reader.Close();
        }
        
        private static void ReadUserDefinedFunctions(SqlCommand command)
        {
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                _userDefinedFunctions.Add(new SqlObject
                (
                    reader["schema"].ToString(),
                    reader["name"].ToString()
                ));
            }
            reader.Close();
        }
    }
}