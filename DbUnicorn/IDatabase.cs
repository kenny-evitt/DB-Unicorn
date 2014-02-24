namespace DbUnicorn
{
    using System.Data;

    public interface IDatabase
    {
        SqlBatchExecution ExecuteSqlBatch(string sql);
        DataTable GetStoredProcedures();
        DataTable GetTable(int tableObjectId);
        DataTable GetTableForeignKeyRelationshipReferencers(string schemaName, string tableName);
        DataTable GetTableForeignKeyRelationshipReferences(string schemaName, string tableName);
        DataTable GetTables();
    }
}
