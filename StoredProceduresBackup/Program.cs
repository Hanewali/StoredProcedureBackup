using System;
using System.Collections.Generic;
// using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace StoredProceduresBackup
{
    public class Program
    {
        private static List<ProcedureObject> _procedureObjects;
        private static List<StoredProcedure> _procedures;
        private static Configuration _configuration;
        private static string _proceduresQuery;
        private static SqlCommand _command;
        private static ServerConnection _serverConnection;
        private static Server _server;
        private static Database _database;

        private void Prepare()
        {
            _procedureObjects = new List<ProcedureObject>();
            _procedures = new List<StoredProcedure>();
            _configuration = new Configuration();
            _proceduresQuery = _configuration.GetProceduresQuery();
            _command = new SqlCommand(_proceduresQuery, _configuration.Connection);
            _serverConnection = new ServerConnection(_configuration.Connection);
            _server = new Server(_serverConnection);
            _database = _server.Databases[_configuration.Connection.Database];
        }
        
        static void Main(string[] args)
        {
            ReadProceduresNames(_command);
            GetProcedures();
            
            
        }

        private static void GetProcedures()
        {
            foreach (var procedureObject in _procedureObjects)
            {
                _procedures.Add(new StoredProcedure(_database, procedureObject.ProcedureName, procedureObject.SchemaName));
            }
        }
        
        private static void ReadProceduresNames(SqlCommand command)
        {
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                _procedureObjects.Add(new ProcedureObject
                (
                    reader["schema"].ToString(),
                    reader["name"].ToString()
                ));
            }

            reader.Close();
        }
        
    }
}