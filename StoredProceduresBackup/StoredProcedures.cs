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
        private string Directory { get; }

        public StoredProcedures()
        {
            Procedures = new List<StoredProcedure>();
            Directory = System.IO.Directory.GetParent(AppContext.BaseDirectory).FullName;
        }

        public void Save()
        {
            SaveToFiles();
            SaveToGit();
        }

        private void SaveToFiles()
        {
            Console.WriteLine("Saving to files...");
            foreach (var procedure in Procedures.Where(procedure => procedure.Schema != "sys"))
            {
                procedure.Refresh();
                var content = procedure.TextHeader + procedure.TextBody;
                var path = $"{Directory}/StoredProcedures/{procedure.Schema}__{procedure.Name}.sql";
                File.WriteAllText(path, content);
            }
        }

        private void SaveToGit()
        {
            Console.WriteLine("Saving to git...");
            using (PowerShell powerShell = PowerShell.Create())
            {
                powerShell.AddScript($"cd {Directory}/StoredProcedures/");
                powerShell.AddScript(@"git add *");
                powerShell.AddScript($"git commit -m 'Timestamp {DateTime.Now.ToShortDateString()} {DateTime.Now.Hour}:{DateTime.Now.Minute}'");
                Collection<PSObject> results = powerShell.Invoke();
            }
        }
    }
}