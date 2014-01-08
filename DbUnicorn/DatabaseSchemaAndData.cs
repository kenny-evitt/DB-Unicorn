namespace DbUnicorn
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;

    using DatabaseObjects;

    public class DatabaseSchemaAndData
    {
        private readonly string _connectionString;
        private readonly IDatabase _database;
        
        public DatabaseSchemaAndData(string connectionString)
        {
            _connectionString = connectionString;
            _database = (IDatabase)(new SqlServerDatabase(connectionString));
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

            using (DataTable data = _database.GetTableForeignKeyRelationshipReferences(table.Schema.Name, table.Name))
            {
                references = (from row in data.AsEnumerable()
                              select new TableRelationship(
                                  tableReferenceLevel,
                                  table,
                                  new Table(
                                      (int)(row["ObjectId"]),
                                      new Schema((string)(row["SchemaName"])),
                                      (string)(row["ObjectName"]),
                                      null))
                            ).ToList<TableRelationship>();
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
            List<TableRelationship> referencers = new List<TableRelationship>();

            using (DataTable data = _database.GetTableForeignKeyRelationshipReferencers(table.Schema.Name, table.Name))
            {
                referencers = (from row in data.AsEnumerable()
                               select new TableRelationship(
                                   tableReferenceLevel,
                                   table,
                                   new Table(
                                       (int)(row["ObjectId"]),
                                       new Schema((string)(row["SchemaName"])),
                                       (string)(row["ObjectName"]),
                                       null))
                            ).ToList<TableRelationship>();
            }

            return referencers;
        }

        /// <summary>
        /// Get the full tree of all tables referenced-by or referencing the given table via a
        /// foreign key relationship, directly or indirectly (i.e. via a third table).
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public List<TableRelationship> GenerateTableForeignKeyRelationshipTree(Table table)
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

        public string GenerateTableForeignKeyRelationshipTreeAsDot(Table table)
        {
            return table.GenerateForeignKeyRelationshipsAsDot(GenerateTableForeignKeyRelationshipTree(table));
        }

        public string GenerateTableForeignKeyRelationshipTreeAsDot(int tableObjectId)
        {
            Table table = this.GetTable(tableObjectId);
            
            return table.GenerateForeignKeyRelationshipsAsDot(GenerateTableForeignKeyRelationshipTree(table));
        }

        public Table GetTable(int tableObjectId)
        {
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
                    if (reader.Read())
                    {
                        return new Table(
                            tableObjectId,
                            new Schema((string)reader["SchemaName"]),
                            (string)reader["TableName"],
                            null);
                    }
                }

                dbConnection.Close();
            }

            throw new ApplicationException(String.Format("No table found for object ID {0}", tableObjectId));
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
    }
}
