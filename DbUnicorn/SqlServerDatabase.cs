﻿namespace DbUnicorn
{
    using System.Data;
    using System.Data.SqlClient;

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
    }
}