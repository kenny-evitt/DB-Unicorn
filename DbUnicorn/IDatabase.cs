namespace DbUnicorn
{
    using System.Collections.Generic;
    using System.Data;

    public interface IDatabase
    {
        List<SqlServerScript> CreateObjectsFromScripts(string scriptsRootFolderPath);

        SqlBatchExecution ExecuteSqlBatch(ISqlBatch sqlBatch);

        DataTable GetStoredProcedures();
        DataTable GetTable(int tableObjectId);
        DataTable GetTableForeignKeyRelationshipReferencers(string schemaName, string tableName);
        DataTable GetTableForeignKeyRelationshipReferences(string schemaName, string tableName);
        DataTable GetTables();
    }
}
