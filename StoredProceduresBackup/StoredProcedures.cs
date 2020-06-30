using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using Microsoft.SqlServer.Management.Smo;

namespace StoredProceduresBackup
{
    public class StoredProcedures
    {
        public List<StoredProcedure> Procedures { get; }
        private string DirectoryPath { get; }
        private string DatabaseName { get; }

        public StoredProcedures(string databaseName)
        {
            DatabaseName = databaseName;
            Procedures = new List<StoredProcedure>();
            DirectoryPath = Directory.GetParent(AppContext.BaseDirectory).FullName + "/StoredProcedures";
        }

        public void Save()
        {
            SaveToFiles();
            SaveToGit();
        }

        private void SaveToFiles()
        {
            Console.WriteLine($"Saving {DatabaseName} to files...");
            foreach (var procedure in Procedures.Where(x => x.Schema != "sys"))
            {
                procedure.Refresh();
                var content = procedure.TextHeader + procedure.TextBody;
                
                if (!Directory.Exists(DirectoryPath))
                    Directory.CreateDirectory(DirectoryPath);
                
                if (!Directory.Exists($"{DirectoryPath}/{DatabaseName}"))
                    Directory.CreateDirectory($"{DirectoryPath}/{DatabaseName}");
                
                if (!Directory.Exists($"{DirectoryPath}/{DatabaseName}/{procedure.Schema}"))
                    Directory.CreateDirectory($"{DirectoryPath}/{DatabaseName}/{procedure.Schema}");
                
                var path = $"{DirectoryPath}/{DatabaseName}/{procedure.Schema}/{procedure.Name}.sql";
                File.WriteAllText(path, content);
            }
        }

        private void SaveToGit()
        {
            Console.WriteLine($"Saving {DatabaseName} to git...");
            using var powerShell = PowerShell.Create();
            powerShell.AddScript($"cd {DirectoryPath}/{DatabaseName}");
            powerShell.AddScript(@"git init");
            powerShell.AddScript(@"git add *");
            powerShell.AddScript($"git commit -m 'Timestamp {DateTime.Now.ToShortDateString()} {DateTime.Now.Hour}:{DateTime.Now.Minute}'");
            powerShell.Invoke();
        }
    }
}