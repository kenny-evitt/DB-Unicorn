namespace Tests.TestDatabases
{
    using System;
    using System.Data;

    using DbUnicorn;
    using DbUnicorn.DatabaseObjects;
    
    public class ForeignKeyRelationshipTreeDatabase : IDatabase
    {
        // Private fields

        private const string _schemaName = "test";

        private const string _backwardReferenceTableName = "BackwardReferenceTable";
        private const string _forwardReferenceTableName = "ForwardReferenceTable";
        private const string _rootTableName = "RootTable";


        // Properties

        public Table BackwardReferenceTable
        {
            get
            {
                return new Table(0, new Schema(_schemaName), _backwardReferenceTableName, null);
            }
        }
        
        public Table RootTable
        {
            get
            {
                return new Table(0, new Schema(_schemaName), _rootTableName, null);
            }
        }

        
        // Public methods

        public void ExecuteSql(string sql)
        {
            throw new NotImplementedException();
        }

        public DataTable GetStoredProcedures()
        {
            throw new NotImplementedException();
        }

        public DataTable GetTable(int tableObjectId)
        {
            throw new NotImplementedException();
        }

        public DataTable GetTableForeignKeyRelationshipReferencers(string schemaName, string tableName)
        {
            if (schemaName == _schemaName && tableName == _rootTableName)
            {
                DataTable references = DatabaseHelper.GetTableRelationshipDataTable();

                DataRow reference = references.NewRow();
                reference["ObjectId"] = 0;
                reference["SchemaName"] = _schemaName;
                reference["ObjectName"] = _backwardReferenceTableName;

                references.Rows.Add(reference);

                return references;
            }
            else
            {
                return new DataTable();
            }
        }

        public DataTable GetTableForeignKeyRelationshipReferences(string schemaName, string tableName)
        {
            if (schemaName == _schemaName && tableName == _rootTableName)
            {
                DataTable references = DatabaseHelper.GetTableRelationshipDataTable();

                DataRow reference = references.NewRow();
                reference["ObjectId"] = 0;
                reference["SchemaName"] = _schemaName;
                reference["ObjectName"] = _forwardReferenceTableName;

                references.Rows.Add(reference);

                return references;
            }
            else
            {
                return new DataTable();
            }
        }

        public DataTable GetTables()
        {
            throw new NotImplementedException();
        }
    }
}
