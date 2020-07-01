using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using Microsoft.SqlServer.Management.Smo;

namespace StoredProceduresBackup
{
    public class SqlObjects
    {
        public List<StoredProcedure> Procedures { get; }
        public List<UserDefinedFunction> Functions { get; }
        private string DirectoryPath { get; }
        private string DatabaseName { get; }

        public SqlObjects(string databaseName)
        {
            DatabaseName = databaseName;
            Procedures = new List<StoredProcedure>();
            DirectoryPath = Directory.GetParent(AppContext.BaseDirectory).FullName + "/SavedObjects";
        }

        public void Save()
        {
            PrepareDirectories();
            SaveObjectsToFiles();
            SaveToGit();
        }

        private void SaveObjectsToFiles()
        {
            Console.WriteLine($"Saving {DatabaseName} to files...");
            SaveProceduresToFiles();
            SaveFunctionsToFiles();
        }

        private void PrepareDirectories()
        {
            if (!Directory.Exists(DirectoryPath))
                Directory.CreateDirectory(DirectoryPath);
                
            if (!Directory.Exists($"{DirectoryPath}/{DatabaseName}"))
                Directory.CreateDirectory($"{DirectoryPath}/{DatabaseName}");

            if (!Directory.Exists($"{DirectoryPath}/{DatabaseName}/StoredProcedures"))
                Directory.CreateDirectory($"{DirectoryPath}/{DatabaseName}/StoredProcedures");
            
            if (!Directory.Exists($"{DirectoryPath}/{DatabaseName}/UserDefinedFunctions"))
                Directory.CreateDirectory($"{DirectoryPath}/{DatabaseName}/UserDefinedFunctions");
            
            if (!Directory.Exists($"{DirectoryPath}/{DatabaseName}/UserDefinedFunctions/ScalarFunctions"))
                Directory.CreateDirectory($"{DirectoryPath}/{DatabaseName}/UserDefinedFunctions/ScalarFunctions");
            
            if (!Directory.Exists($"{DirectoryPath}/{DatabaseName}/UserDefinedFunctions/TableFunctions"))
                Directory.CreateDirectory($"{DirectoryPath}/{DatabaseName}/UserDefinedFunctions/TableFunctions");
        }

        private void SaveFunctionsToFiles()
        {
            foreach (var function in Functions.Where(x => x.Schema != "sys"))
            {
                function.Refresh();
                var content = function.TextHeader + function.TextBody;

                switch (function.FunctionType)
                {
                    case UserDefinedFunctionType.Scalar:
                        if(!Directory.Exists($"{DirectoryPath}/{DatabaseName}/UserDefinedFunctions/ScalarFunctions/{function.Schema}"))
                            Directory.CreateDirectory($"{DirectoryPath}/{DatabaseName}/UserDefinedFunctions/ScalarFunctions/{function.Schema}");
    
                        var scalarPath =
                            $"{DirectoryPath}/{DatabaseName}/UserDefinedFunctions/ScalarFunctions/{function.Schema}/{function.Name}.sql";
                        File.WriteAllText(scalarPath, content);
                        
                        break;
                    
                    case UserDefinedFunctionType.Table:
                        if(!Directory.Exists($"{DirectoryPath}/{DatabaseName}/UserDefinedFunctions/TableFunctions/{function.Schema}"))
                            Directory.CreateDirectory($"{DirectoryPath}/{DatabaseName}/UserDefinedFunctions/TableFunctions/{function.Schema}");
    
                        var tablePath =
                            $"{DirectoryPath}/{DatabaseName}/UserDefinedFunctions/TableFunctions/{function.Schema}/{function.Name}.sql";
                        File.WriteAllText(tablePath, content);
                        
                        break;
                    
                    case UserDefinedFunctionType.Inline:
                        Console.WriteLine("eh?");
                        break;
                }
            }
        }

        private void SaveProceduresToFiles()
        {
            foreach (var procedure in Procedures.Where(x => x.Schema != "sys"))
            {
                procedure.Refresh();
                var content = procedure.TextHeader + procedure.TextBody;
                
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