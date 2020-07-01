namespace StoredProceduresBackup
{
    public class SqlObject
    {
        public string SchemaName { get; }
        public string Name { get; }

        public SqlObject(string schemaName, string name)
        {
            SchemaName = schemaName;
            Name = name;
        }
    }
}