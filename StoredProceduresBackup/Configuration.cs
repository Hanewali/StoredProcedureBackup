using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Namotion.Reflection;
using Newtonsoft.Json;

namespace StoredProceduresBackup
{
    public class Configuration
    {
        private IConfigurationRoot _configuration;
        public List<string> ConnectionStrings;

        public Configuration()
        {
            ServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
        }

        private void ConfigureServices(IServiceCollection serviceCollection)
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();

            serviceCollection.AddSingleton(_configuration);
            GetConnectionStrings();
        }
        
        private void GetConnectionStrings()
        {
            ConnectionStrings = JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText(Directory.GetParent(AppContext.BaseDirectory).FullName + "/appsettings.json")).ConnectionStrings.Values.ToList();
        }
        
        public string GetProceduresQuery()
        {
            return File.ReadAllText(Directory.GetParent(AppContext.BaseDirectory).FullName + "/GetProcedures.sql");
        }
    }
}