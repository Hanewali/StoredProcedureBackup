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
        private static SqlCommand _command;
        private static ServerConnection _serverConnection;
        private static Server _server;
        private static Database _database;
        private static StoredProcedures _storedProcedures;
        private static SqlConnection _connection;
        
        private static void Prepare()
        {
            _sqlObjects = new List<SqlObject>();
            _configuration = new Configuration();
            _proceduresQuery = _configuration.GetProceduresQuery();
        }

        private static void PrepareConnection(string connectionString)
        {
            _connection = new SqlConnection(connectionString);
            _connection.Open();
            _command = new SqlCommand(_proceduresQuery, _connection);
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
                ReadSqlObjects(_command);
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
            _sqlObjects = new List<SqlObject>();   
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