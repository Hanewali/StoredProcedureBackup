namespace StoredProceduresBackup
{
    public class SqlObject
    {
        public string SchemaName { get; }
        public string Type { get; }
        public string Name { get; }

        public SqlObject(string schemaName, string name)
        {
            SchemaName = schemaName;
            Name = name;
            }
        public SqlObject(string schemaName, string name, string type)
        {
            SchemaName = schemaName;
            Name = name;
            Type = type;
        }
    }
}