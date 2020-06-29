using System.Collections.Generic;

namespace StoredProceduresBackup
{
    public class AppSettings
    {
        // public Logging Logging { get; set; }
        public Dictionary<string, Dictionary<string,string>> Logging { get; set; }
        public Dictionary<string, string> ConnectionStrings { get; set; }
        
    }

    public class Logging
    {
        public List<Dictionary<string,string>> LogLevel { get; set; }
        // public LogLevel LogLevel { get; set; }
    }

    public class LogLevel
    {
        public string Default { get; set; }
        public string System { get; set; }
        public string Information { get; set; }
    }

    class ConnectionStrings
    {
        
    }
}