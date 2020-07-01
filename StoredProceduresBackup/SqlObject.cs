namespace StoredProceduresBackup
{
    public class SqlObject
    {
        public string SchemaName { get; }
        public string ProcedureName { get; }

        public SqlObject(string schemaName, string procedureName)
        {
            SchemaName = schemaName;
            ProcedureName = procedureName;
        }
    }
}