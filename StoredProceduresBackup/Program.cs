using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace StoredProceduresBackup
{
    public static class Program
    {
        private static List<SqlObject> _sqlObjects;
        private static Configuration _configuration;
        private static string _proceduresQuery;
        private static string _functionsQuery;
        private static SqlCommand _proceduresCommand;
        private static ServerConnection _serverConnection;
        private static Server _server;
        private static Database _database;
        private static StoredProcedures _storedProcedures;
        private static SqlConnection _connection;
        private static SqlCommand _functionsCommand;

        private static void Prepare()
        {
            _sqlObjects = new List<SqlObject>();
            _configuration = new Configuration();
            _sqlObjects = new List<SqlObject>();   
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
            _storedProcedures = new StoredProcedures(_database.Name);
        }
        
        static void Main()
        {
            Prepare();

            foreach (var connectionString in _configuration.ConnectionStrings)
            {
                PrepareConnection(connectionString);
                ReadSqlObjects(_proceduresCommand);
                ReadSqlObjects(_functionsCommand);
                GetProcedures();
                _storedProcedures.Save(); 
            }
        }

        private static void GetProcedures()
        {
            foreach (var procedureObject in _sqlObjects)
            {
                _storedProcedures.Procedures.Add(new StoredProcedure(_database, procedureObject.ProcedureName, procedureObject.SchemaName));
            }
        }
        
        private static void ReadSqlObjects(SqlCommand command)
        {
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                _sqlObjects.Add(new SqlObject
                (
                    reader["schema"].ToString(),
                    reader["name"].ToString()
                ));
            }
            reader.Close();
        }
    }
}