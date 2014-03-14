namespace DbUnicorn
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;

    public class SqlServerDatabase : IDatabase
    {
        // Private fields
        
        private string _connectionString;
        

        // Constructors

        public SqlServerDatabase(string connectionString)
        {
            _connectionString = connectionString;
        }


        // Public methods

        public List<SqlServerScript> CreateObjectsFromScripts(string scriptsRootFolderPath)
        {
            List<SqlServerScript> scripts = new List<SqlServerScript>();

            scripts.AddRange(this.CreateSchemasFromScripts(scriptsRootFolderPath));
            scripts.AddRange(this.CreateSynonymsFromScripts(scriptsRootFolderPath));
            scripts.AddRange(this.CreateUserDefinedDataTypesFromScripts(scriptsRootFolderPath));
            scripts.AddRange(this.CreateTablesFromScripts(scriptsRootFolderPath));
            scripts.AddRange(this.CreateOtherObjectsFromScripts(scriptsRootFolderPath));

            return scripts;
        }

        public SqlBatchExecution ExecuteSqlBatch(ISqlBatch sqlBatch)
        {
            SqlBatchExecution execution = new SqlBatchExecution();
            
            using (SqlConnection dbConnection = new SqlConnection(_connectionString))
            using (SqlCommand dbSqlCommand = new SqlCommand(sqlBatch.Sql, dbConnection))
            {
                dbSqlCommand.CommandType = CommandType.Text;
                dbConnection.Open();

                try
                {
                    dbSqlCommand.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    execution.Exception = ex;
                }

                dbConnection.Close();
            }

            sqlBatch.Executions.Add(execution);

            return execution;
        }

        public DataTable GetStoredProcedures()
        {
            DataTable storedProcedures = new DataTable();

            using (SqlConnection dbConnection = new SqlConnection(_connectionString))
            using (SqlCommand dbSelectCommand = new SqlCommand(@"SELECT	SchemaName = s.name,
		ProcedureName = p.name,
		ProcedureText = m.[definition],
		ProcedureUsesAnsiNulls = m.uses_ansi_nulls,
		ProcedureUsesQuotedIdentifiers = m.uses_quoted_identifier
FROM	sys.procedures p
		JOIN sys.schemas s ON p.[schema_id] = s.[schema_id]
		JOIN sys.sql_modules m ON p.[object_id] = m.[object_id]
ORDER BY SchemaName, ProcedureName;", dbConnection))
            {
                dbSelectCommand.CommandType = CommandType.Text;
                dbConnection.Open();

                using (SqlDataReader reader = dbSelectCommand.ExecuteReader())
                {
                    storedProcedures.Load(reader);
                }

                dbConnection.Close();
            }

            return storedProcedures;
        }

        public DataTable GetTable(int tableObjectId)
        {
            DataTable table = new DataTable();

            using (SqlConnection dbConnection = new SqlConnection(_connectionString))
            using (SqlCommand dbSelectCommand = new SqlCommand(@"SELECT	SchemaName = s.name,
		TableName = t.name
FROM	sys.tables t
		JOIN sys.schemas s ON t.[schema_id] = s.[schema_id]
WHERE t.[object_id] = @tableObjectId
ORDER BY SchemaName, TableName;", dbConnection))
            {
                dbSelectCommand.CommandType = CommandType.Text;
                dbSelectCommand.Parameters.AddWithValue("tableObjectId", tableObjectId);
                dbConnection.Open();

                using (SqlDataReader reader = dbSelectCommand.ExecuteReader())
                {
                    table.Load(reader);
                }

                dbConnection.Close();
            }

            return table;
        }

        public DataTable GetTables()
        {
            DataTable tables = new DataTable();

            using (SqlConnection dbConnection = new SqlConnection(_connectionString))
            using (SqlCommand dbSelectCommand = new SqlCommand(@"SELECT	TableObjectId = t.[object_id],
		SchemaName = s.name,
		TableName = t.name
FROM	sys.tables t
		JOIN sys.schemas s ON t.[schema_id] = s.[schema_id]
ORDER BY SchemaName, TableName;", dbConnection))
            {
                dbSelectCommand.CommandType = CommandType.Text;
                dbConnection.Open();

                using (SqlDataReader reader = dbSelectCommand.ExecuteReader())
                {
                    tables.Load(reader);
                }

                dbConnection.Close();
            }

            return tables;
        }

        public DataTable GetTableForeignKeyRelationshipReferencers(string schemaName, string tableName)
        {
            DataTable referencers = new DataTable();

            using (SqlConnection dbConnection = new SqlConnection(_connectionString))
            using (SqlCommand dbSelectCommand = new SqlCommand(@"SELECT	SchemaName = o_parent_schema.[name],
		ObjectId = o_parent.[object_id],
		ObjectName = o_parent.[name]
FROM	sys.foreign_keys fk
		JOIN sys.objects o_parent ON fk.parent_object_id = o_parent.object_id
		JOIN sys.schemas o_parent_schema ON o_parent.schema_id = o_parent_schema.schema_id
		JOIN sys.objects o_referenced ON fk.referenced_object_id = o_referenced.object_id
		JOIN sys.schemas o_referenced_schema ON o_referenced.schema_id = o_referenced_schema.schema_id
WHERE	o_referenced_schema.[name] = @objectSchemaName
		AND o_referenced.[name] = @objectName;", dbConnection))
            {
                dbSelectCommand.CommandType = CommandType.Text;
                dbSelectCommand.Parameters.AddWithValue("objectSchemaName", schemaName);
                dbSelectCommand.Parameters.AddWithValue("objectName", tableName);
                dbConnection.Open();

                using (SqlDataReader reader = dbSelectCommand.ExecuteReader())
                {
                    referencers.Load(reader);
                }

                dbConnection.Close();
            }

            return referencers;
        }

        public DataTable GetTableForeignKeyRelationshipReferences(string schemaName, string tableName)
        {
            DataTable references = new DataTable();
            
            using (SqlConnection dbConnection = new SqlConnection(_connectionString))
            using (SqlCommand dbSelectCommand = new SqlCommand(@"SELECT	SchemaName = o_referenced_schema.[name],
		ObjectId = o_referenced.[object_id],
		ObjectName = o_referenced.[name]
FROM	sys.foreign_keys fk
		JOIN sys.objects o_parent ON fk.parent_object_id = o_parent.object_id
		JOIN sys.schemas o_parent_schema ON o_parent.schema_id = o_parent_schema.schema_id
		JOIN sys.objects o_referenced ON fk.referenced_object_id = o_referenced.object_id
		JOIN sys.schemas o_referenced_schema ON o_referenced.schema_id = o_referenced_schema.schema_id
WHERE	o_parent_schema.[name] = @objectSchemaName
		AND o_parent.[name] = @objectName;", dbConnection))
            {
                dbSelectCommand.CommandType = CommandType.Text;
                dbSelectCommand.Parameters.AddWithValue("objectSchemaName", schemaName);
                dbSelectCommand.Parameters.AddWithValue("objectName", tableName);
                dbConnection.Open();

                using (SqlDataReader reader = dbSelectCommand.ExecuteReader())
                {
                    references.Load(reader);
                }

                dbConnection.Close();
            }

            return references;
        }


        // Private methods

        private List<SqlServerScript> CreateOtherObjectsFromScripts(string scriptsRootFolderPath)
        {
            List<string> otherObjectsScriptsFolders =
                new List<string>
                {
                    "Stored Procedures",
                    "Triggers",
                    "User Defined Functions",
                    "Views"
                };

            IEnumerable<string> otherObjectsScriptsFolderPaths =
                (from folder in otherObjectsScriptsFolders
                 select Path.Combine(scriptsRootFolderPath, folder));

            return SqlServerScriptExecutor.ExecuteScriptsInFolders(otherObjectsScriptsFolderPaths, this, null);
        }
        
        private List<SqlServerScript> CreateSchemasFromScripts(string scriptsRootFolderPath)
        {
            string schemasScriptsFolderPath = Path.Combine(scriptsRootFolderPath, "Schemas");

            return SqlServerScriptExecutor.ExecuteScriptsInFolder(schemasScriptsFolderPath, this, 0);
        }

        private List<SqlServerScript> CreateSynonymsFromScripts(string scriptsRootFolderPath)
        {
            string synonymsScriptsFolderPath = Path.Combine(scriptsRootFolderPath, "Synonyms");

            return SqlServerScriptExecutor.ExecuteScriptsInFolder(synonymsScriptsFolderPath, this, 0);
        }
        
        private List<SqlServerScript> CreateTablesFromScripts(string scriptsRootFolderPath)
        {
            string tableScriptsFolderPath = Path.Combine(scriptsRootFolderPath, "Tables");
            
            return SqlServerScriptExecutor.ExecuteScriptsInFolder(tableScriptsFolderPath, this, 1);
        }

        private List<SqlServerScript> CreateUserDefinedDataTypesFromScripts(string scriptsRootFolderPath)
        {
            string userDefinedDataTypesScriptsFolderPath = Path.Combine(scriptsRootFolderPath, "User Defined Data Types");

            return SqlServerScriptExecutor.ExecuteScriptsInFolder(userDefinedDataTypesScriptsFolderPath, this, 0);
        }
    }
}
