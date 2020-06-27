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
        private static IConfigurationRoot _configuration;
        private static SqlConnection _connection;
        private static string _proceduresQuery;

        static void Main(string[] args)
        {
            //start
            ServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            _connection.Open();

            ConfigureQueries();

            var command = new SqlCommand(_proceduresQuery, _connection);

            var procedureObjects = new List<ProcedureObject>();

            var procedures = new List<StoredProcedure>();

            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                procedureObjects.Add(new ProcedureObject
                (
                    reader["schema"].ToString(),
                    reader["name"].ToString()
                ));
            }

            reader.Close();

            var srvCon = new ServerConnection(_connection);
            var server = new Server(srvCon);
            var dataBase = server.Databases[_connection.Database];

            foreach (var procedureObject in procedureObjects)
            {
                procedures.Add(new StoredProcedure(dataBase, procedureObject.ProcedureName,
                    procedureObject.SchemaName));
            }

            foreach (var procedure in procedures)
            {
                procedure.Refresh();
                Console.WriteLine(procedure.TextHeader.Replace("CREATE", "ALTER").Replace("create", "ALTER"));
                Console.WriteLine(procedure.TextBody);
            }
        }

        private static void ConfigureQueries()
        {
            _proceduresQuery =
                File.ReadAllText(Directory.GetParent(AppContext.BaseDirectory).FullName + "/GetProcedures.sql");
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();

            serviceCollection.AddSingleton(_configuration);

            _connection = new SqlConnection(GetConnectionString());
        }

        private static string GetConnectionString()
        {
            return _configuration.GetConnectionString("MyConnection");
        }
    }
}