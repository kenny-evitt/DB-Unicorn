namespace Tests.TestDatabases
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    using DbUnicorn;

    public class TestDatabase : IDatabase
    {
        public virtual List<SqlServerScript> CreateObjectsFromScripts(string scriptsRootFolderPath)
        {
            throw new NotImplementedException();
        }

        public virtual SqlBatchExecution ExecuteSqlBatch(ISqlBatch sqlBatch)
        {
            throw new NotImplementedException();
        }

        public virtual DataTable GetStoredProcedures()
        {
            throw new NotImplementedException();
        }

        public virtual DataTable GetTable(int tableObjectId)
        {
            throw new NotImplementedException();
        }

        public virtual DataTable GetTableForeignKeyRelationshipReferencers(string schemaName, string tableName)
        {
            throw new NotImplementedException();
        }

        public virtual DataTable GetTableForeignKeyRelationshipReferences(string schemaName, string tableName)
        {
            throw new NotImplementedException();
        }

        public virtual DataTable GetTables()
        {
            throw new NotImplementedException();
        }
    }
}
