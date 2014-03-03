namespace Tests.TestDatabases
{
    using System;
    using System.Data;
    
    using DbUnicorn;
    using DbUnicorn.DatabaseObjects;

    public class SelfReferencingTableDatabase : TestDatabase
    {
        // Private fields

        private const string _schemaName = "test";
        private const string _tableName = "SelfReferencingTable";


        // Properties

        public Table Table
        {
            get
            {
                return new Table(0, new Schema(_schemaName), _tableName, null);
            }
        }


        // Public methods

        new public DataTable GetTableForeignKeyRelationshipReferencers(string schemaName, string tableName)
        {
            if (schemaName == _schemaName && tableName == _tableName)
            {
                return GenerateSelfReferenceRelationshipDataTable();
            }
            else
            {
                throw new ArgumentException(
                    String.Format(
                        "The specified schema name and table name don't identify the supported test self-referencing table ({0}.{1}).",
                        _schemaName,
                        _tableName));
            }
        }

        new public DataTable GetTableForeignKeyRelationshipReferences(string schemaName, string tableName)
        {
            if (schemaName == _schemaName && tableName == _tableName)
            {
                return GenerateSelfReferenceRelationshipDataTable();
            }
            else
            {
                throw new ArgumentException(
                    String.Format(
                        "The specified schema name and table name don't identify the supported test self-referencing table ({0}.{1}).",
                        _schemaName,
                        _tableName));
            }
        }


        // Private methods

        private DataTable GenerateSelfReferenceRelationshipDataTable()
        {
            DataTable table = DatabaseHelper.GetTableRelationshipDataTable();

            DataRow row;

            row = table.NewRow();
            row["ObjectId"] = 0;
            row["SchemaName"] = _schemaName;
            row["ObjectName"] = _tableName;
            table.Rows.Add(row);

            return table;
        }
    }
}
