namespace StoredProceduresBackup
{
    public class ProcedureObject
    {
        public string SchemaName { get; }
        public string ProcedureName { get; }

        public ProcedureObject(string schemaName, string procedureName)
        {
            SchemaName = schemaName;
            ProcedureName = procedureName;
        }
    }
}