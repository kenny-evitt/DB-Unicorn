namespace DbUnicorn
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    using DatabaseObjects;

    public class DatabaseSchemaAndData
    {
        private readonly string _connectionString;
        
        public DatabaseSchemaAndData(string connectionString)
        {
            _connectionString = connectionString;
        }


        // Public methods

        public List<StoredProcedure> GetStoredProcedures()
        {
            List<StoredProcedure> storedProcedures = new List<StoredProcedure>();

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
                    while (reader.Read())
                    {
                        storedProcedures.Add(
                            new StoredProcedure(
                                new Schema(
                                    (string)reader["SchemaName"]),
                                    (string)reader["ProcedureName"],
                                    (string)reader["ProcedureText"],
                                    (bool)reader["ProcedureUsesAnsiNulls"],
                                    (bool)reader["ProcedureUsesQuotedIdentifiers"]));
                    }
                }

                dbConnection.Close();
            }

            return storedProcedures;
        }

        /// <summary>
        /// Get tables referenced by the given table via a foreign key relationship.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="tableReferenceLevel">This is an optional parameter used to construct the
        /// table relationship objects returned by this method when generating multi-level 'trees'
        /// of relationships.</param>
        /// <returns></returns>
        public List<TableRelationship> GetTableForeignKeyRelationshipReferences(Table table, int tableReferenceLevel = 1)
        {
            List<TableRelationship> references = new List<TableRelationship>();

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
                dbSelectCommand.Parameters.AddWithValue("objectSchemaName", table.Schema.Name);
                dbSelectCommand.Parameters.AddWithValue("objectName", table.Name);
                dbConnection.Open();

                using (SqlDataReader reader = dbSelectCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        references.Add(
                            new TableRelationship(
                                tableReferenceLevel,
                                table,
                                new Table(
                                    (int)reader["ObjectId"],
                                    new Schema((string)reader["SchemaName"]),
                                    (string)reader["ObjectName"],
                                    null)));
                    }
                }

                dbConnection.Close();
            }

            return references;
        }

        /// <summary>
        /// Get tables that reference the given table via a foreign key relationship.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="tableReferenceLevel">This is an optional parameter used to construct the
        /// table relationship objects returned by this method when generating multi-level 'trees'
        /// of relationships.</param>
        /// <returns></returns>
        public List<TableRelationship> GetTableForeignKeyRelationshipReferencers(Table table, int tableReferenceLevel = -1)
        {
            List<TableRelationship> references = new List<TableRelationship>();

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
                dbSelectCommand.Parameters.AddWithValue("objectSchemaName", table.Schema.Name);
                dbSelectCommand.Parameters.AddWithValue("objectName", table.Name);
                dbConnection.Open();

                using (SqlDataReader reader = dbSelectCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        references.Add(
                            new TableRelationship(
                                tableReferenceLevel,
                                new Table(
                                    (int)reader["ObjectId"],
                                    new Schema((string)reader["SchemaName"]),
                                    (string)reader["ObjectName"],
                                    null),
                                table));
                    }
                }

                dbConnection.Close();
            }

            return references;
        }

        public string GenerateTableForeignKeyRelationshipTreeAsDot(Table table)
        {
            return table.GenerateForeignKeyRelationshipsAsDot(GenerateTableForeignKeyRelationshipTree(table));
        }

        public List<Table> GetTables()
        {
            List<Table> tables = new List<Table>();

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
                    while (reader.Read())
                    {
                        tables.Add(
                            new Table(
                                (int)reader["TableObjectId"],
                                new Schema((string)reader["SchemaName"]),
                                (string)reader["TableName"],
                                null));
                    }
                }

                dbConnection.Close();
            }

            return tables;
        }





        // Private methods



        /// <summary>
        /// Get the full tree of all tables referenced-by or referencing the given table via a
        /// foreign key relationship, directly or indirectly (i.e. via a third table).
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private List<TableRelationship> GenerateTableForeignKeyRelationshipTree(Table table)
        {
            List<TableRelationship> relationshipTree = new List<TableRelationship>();


            // Determine all of the 'forward' references, i.e. the tree of objects that *are referenced by* the given object.

            int currentReferenceLevel = 1;
            List<TableRelationship> currentLevelReferences = GetTableForeignKeyRelationshipReferences(table);
            List<TableRelationship> nextLevelReferences = new List<TableRelationship>();

            do
            {
                nextLevelReferences.Clear();

                foreach (TableRelationship reference in currentLevelReferences)
                {
                    // If the reference is new, add all of its references to the tree.

                    if ((!(table.Schema.Name == reference.ReferencedTable.Schema.Name && table.Name == reference.ReferencedTable.Name))
                        && (!relationshipTree.Exists(x => x.ReferencedTable.Schema.Name == reference.ReferencedTable.Schema.Name && x.ReferencedTable.Name == reference.ReferencedTable.Name))
                        && (!nextLevelReferences.Exists(x => x.BaseTable.Schema.Name == reference.ReferencedTable.Schema.Name && x.BaseTable.Name == reference.ReferencedTable.Name)))
                    {
                        nextLevelReferences.AddRange(GetTableForeignKeyRelationshipReferences(reference.ReferencedTable, currentReferenceLevel + 1));
                    }
                }

                relationshipTree.AddRange(currentLevelReferences);
                currentReferenceLevel++;
                currentLevelReferences.Clear();
                currentLevelReferences.AddRange(nextLevelReferences);
            } while (nextLevelReferences.Count > 1);


            // Determine all of the 'backward' references, i.e. the tree of objects that *reference* the given object.

            List<TableRelationship> backwardReferences = new List<TableRelationship>();
            currentReferenceLevel = -1;
            currentLevelReferences = GetTableForeignKeyRelationshipReferencers(table);

            do
            {
                nextLevelReferences.Clear();

                foreach (TableRelationship reference in currentLevelReferences)
                {
                    // If the referencer is new, add all of its referencers to the tree.

                    if ((!(table.Schema.Name == reference.BaseTable.Schema.Name && table.Name == reference.BaseTable.Name))
                        && (!backwardReferences.Exists(x => x.BaseTable.Schema.Name == reference.BaseTable.Schema.Name && x.BaseTable.Name == reference.BaseTable.Name))
                        && (!nextLevelReferences.Exists(x => x.ReferencedTable.Schema.Name == reference.BaseTable.Schema.Name && x.ReferencedTable.Name == reference.BaseTable.Name)))
                    {
                        nextLevelReferences.AddRange(GetTableForeignKeyRelationshipReferencers(reference.BaseTable, currentReferenceLevel - 1));
                    }
                }

                backwardReferences.AddRange(currentLevelReferences);
                currentReferenceLevel--;
                currentLevelReferences.Clear();
                currentLevelReferences.AddRange(nextLevelReferences);
            } while (nextLevelReferences.Count > 1);


            relationshipTree.AddRange(backwardReferences);

            return relationshipTree;
        }
    }
}
