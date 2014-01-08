namespace DbUnicorn
{
    using System.Data;

    public interface IDatabase
    {
        DataTable GetTableForeignKeyRelationshipReferencers(string schemaName, string tableName);
        DataTable GetTableForeignKeyRelationshipReferences(string schemaName, string tableName);
    }
}
