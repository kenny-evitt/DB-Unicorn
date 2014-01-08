namespace DbUnicorn
{
    using System.Data;

    public interface IDatabase
    {
        DataTable GetTableForeignKeyRelationshipReferences(string schemaName, string tableName);
    }
}
