using System.Collections.Generic;

namespace StoredProceduresBackup
{
    public class AppSettings
    {
        public Dictionary<string, Dictionary<string,string>> Logging { get; set; }
        public Dictionary<string, string> ConnectionStrings { get; set; }
    }
}