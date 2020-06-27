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
        private string _directory { get; set; }

        public StoredProcedures()
        {
            Procedures = new List<StoredProcedure>();
            _directory = Directory.GetParent(AppContext.BaseDirectory).FullName;
        }

        public void Save()
        {
            SaveToFiles();
            SaveToGit();
        }

        private void SaveToFiles()
        {
            Console.WriteLine(Directory.GetParent(AppContext.BaseDirectory));
            foreach (var procedure in Procedures)
            {
                procedure.Refresh();
                var content = procedure.TextHeader + procedure.TextBody;
                var path = $"{_directory}/StoredProcedures/{procedure.Name}.sql";
                File.WriteAllText(path, content);
            }
        }

        private void SaveToGit()
        {
            using (PowerShell powerShell = PowerShell.Create())
            {
                powerShell.AddScript($"cd {_directory}/StoredProcedures/");
                powerShell.AddScript(@"git add *");
                powerShell.AddScript($"git commit -m 'Timestamp {DateTime.Now.ToShortDateString()}'");
                Collection<PSObject> results = powerShell.Invoke();
            }
        }
    }
}