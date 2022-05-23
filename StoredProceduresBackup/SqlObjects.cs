using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using Microsoft.SqlServer.Management.Smo;
using StoredProceduresBackup.Dto;

namespace StoredProceduresBackup
{
    public class SqlObjects
    {
        public List<StoredProcedure> Procedures { get; }
        public List<UserDefinedFunctionWithType> Functions { get; }
        private string DirectoryPath { get; }
        private string DatabaseName { get; }

        public SqlObjects(string databaseName, string directoryPath)
        {
            DatabaseName = databaseName;
            Procedures = new List<StoredProcedure>();
            Functions = new List<UserDefinedFunctionWithType>();
            DirectoryPath = directoryPath;
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

            foreach (var type in Functions.Select(x => x.UserDefinedFunctionType).Distinct())
            {
                if (!Directory.Exists($"{DirectoryPath}/{DatabaseName}/UserDefinedFunctions/{GetFunctionTypeName(type)}"))
                    Directory.CreateDirectory($"{DirectoryPath}/{DatabaseName}/UserDefinedFunctions/{GetFunctionTypeName(type)}");
            }
        }

        private void SaveFunctionsToFiles()
        {
            foreach (var function in Functions.Where(x => x.Function.Schema != "sys"))
            {
                function.Function.Refresh();
                var content = function.Function.TextHeader + function.Function.TextBody;

                var fullDirectoryPath =
                    $"{DirectoryPath}/{DatabaseName}/UserDefinedFunctions/{GetFunctionTypeName(function.Function.FunctionType)}/{function.Function.Schema}";
                
                if (!Directory.Exists(fullDirectoryPath))
                    Directory.CreateDirectory(fullDirectoryPath);
                
                var path = $"{fullDirectoryPath}/{function.Function.Name}.sql";
                File.WriteAllText(path, content);
            }
        }

        private void SaveProceduresToFiles()
        {
            foreach (var procedure in Procedures.Where(x => x.Schema != "sys"))
            {
                procedure.Refresh();
                var content = procedure.TextHeader + procedure.TextBody;
                
                if (!Directory.Exists($"{DirectoryPath}/{DatabaseName}/StoredProcedures/{procedure.Schema}"))
                    Directory.CreateDirectory($"{DirectoryPath}/{DatabaseName}/StoredProcedures/{procedure.Schema}");
                
                var path = $"{DirectoryPath}/{DatabaseName}/StoredProcedures/{procedure.Schema}/{procedure.Name}.sql";
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

        private string GetFunctionTypeName(UserDefinedFunctionType type)
        {
            return type switch
            {
                UserDefinedFunctionType.Inline => "Inline_Table_Function",
                UserDefinedFunctionType.Scalar => "Scalar_Function",
                UserDefinedFunctionType.Table => "Multiline_Table_Function",
                UserDefinedFunctionType.Unknown => "Unknown_Function",
                _ => "Unknown_Function"
            };
        }
    }
}