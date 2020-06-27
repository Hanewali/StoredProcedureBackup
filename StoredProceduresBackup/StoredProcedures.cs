using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.SqlServer.Management.Smo;

namespace StoredProceduresBackup
{
    public class StoredProcedures
    {
        public List<StoredProcedure> Procedures { get; set; }

        public StoredProcedures()
        {
            Procedures = new List<StoredProcedure>();
        }

        public void Save()
        {
            SaveToFiles();
            SaveToGit();
        }

        private void SaveToFiles()
        {
            Console.WriteLine(Directory.GetParent(AppContext.BaseDirectory).FullName);
        }

        private void SaveToGit()
        {
            return;
        }

    }
}