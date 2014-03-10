namespace Tests.TestDatabases
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    using DbUnicorn;

    public class TestDatabase : IDatabase
    {
        public List<SqlServerScript> CreateObjectsFromScripts(string scriptsRootFolderPath)
        {
            throw new NotImplementedException();
        }

        public SqlBatchExecution ExecuteSqlBatch(ISqlBatch sqlBatch)
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
            throw new NotImplementedException();
        }

        public DataTable GetTableForeignKeyRelationshipReferences(string schemaName, string tableName)
        {
            throw new NotImplementedException();
        }

        public DataTable GetTables()
        {
            throw new NotImplementedException();
        }
    }
}
