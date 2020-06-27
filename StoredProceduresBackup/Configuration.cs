using System;
using System.IO;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace StoredProceduresBackup
{
    public class Configuration
    {
        private IConfigurationRoot _configuration;

        public SqlConnection Connection;

        public Configuration()
        {
            ServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            Connection.Open();
        }

        private void ConfigureServices(IServiceCollection serviceCollection)
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();

            serviceCollection.AddSingleton(_configuration);

            Connection = new SqlConnection(GetConnectionString());
        }
        
        private string GetConnectionString()
        {
            return _configuration.GetConnectionString("MyConnection");
        }
        
        public string GetProceduresQuery()
        {
            return File.ReadAllText(Directory.GetParent(AppContext.BaseDirectory).FullName + "/GetProcedures.sql");
        }
    }
}