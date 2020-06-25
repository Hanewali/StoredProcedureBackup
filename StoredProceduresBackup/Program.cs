using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

                var procedures = new List<ProcedureObject>();
                
                SqlDataReader reader = command.ExecuteReader();

                try
                {
                    Console.WriteLine("dupa");
                    while (reader.Read())
                    {
                        procedures.Add(new ProcedureObject
                        {    
                            SchemaName = reader["schema"].ToString(),
                            ProcedureName = reader["name"].ToString()
                        });
                    }

                    foreach (var procedure in procedures)
                    {
                        Console.WriteLine($"{procedure.SchemaName}.{procedure.ProcedureName}");
                    }
                }
                finally
                {
                    reader.Close();
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

        private static void OpenSqlConnection()
        {
            string connectionString = GetConnectionString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.ConnectionString = connectionString;
                connection.Open();
                
                Console.WriteLine($"State: {connection.State}");
                Console.WriteLine($"ConnectionString; {connection.ConnectionString}");
            }
        }

        private static string GetConnectionString()
        {
            return _configuration.GetConnectionString("MyConnection");
        }
    }
}