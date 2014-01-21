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
        private readonly IDatabase _database;
        
        public DatabaseSchemaAndData(IDatabase database)
        {
            _database = database;
        }


        // Public methods

        public List<StoredProcedure> GetStoredProcedures()
        {
            using (DataTable data = _database.GetStoredProcedures())
            {
                return (from row in data.AsEnumerable()
                        select new StoredProcedure(
                            new Schema((string)row["SchemaName"]),
                            (string)row["ProcedureName"],
                            (string)row["ProcedureText"],
                            (bool)row["ProcedureUsesAnsiNulls"],
                            (bool)row["ProcedureUsesQuotedIdentifiers"])
                    ).ToList<StoredProcedure>();
            }
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
            using (DataTable data = _database.GetTableForeignKeyRelationshipReferences(table.Schema.Name, table.Name))
            {
                return (from row in data.AsEnumerable()
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
            using (DataTable data = _database.GetTableForeignKeyRelationshipReferencers(table.Schema.Name, table.Name))
            {
                return (from row in data.AsEnumerable()
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

            // Remove self-references, as they're already covered by 'forward references'.
            currentLevelReferences.RemoveAll(r => table.Schema.Name == r.BaseTable.Schema.Name && table.Name == r.BaseTable.Name);

            do
            {
                nextLevelReferences.Clear();

                foreach (TableRelationship reference in currentLevelReferences)
                {
                    // If the referencer (backward reference) is new, add all of *its* referencers 
                    // to the tree.

                    if ((!backwardReferences.Exists(x => x.BaseTable.Schema.Name == reference.BaseTable.Schema.Name && x.BaseTable.Name == reference.BaseTable.Name))
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
            using (DataTable data = _database.GetTable(tableObjectId))
            {
                if (data.Rows.Count == 0)
                {
                    throw new ApplicationException(String.Format("No table found for object ID {0}", tableObjectId));
                }
                else if (data.Rows.Count == 1)
                {
                    return new Table(
                        tableObjectId,
                        new Schema((string)data.Rows[0]["SchemaName"]),
                        (string)data.Rows[0]["TableName"],
                        null);
                }
                else
                {
                    throw new ApplicationException(
                        String.Format(
                            "Multiple tables were found for object ID {0}!",
                            tableObjectId));
                }
            }
        }

        public List<Table> GetTables()
        {
            using (DataTable data = _database.GetTables())
            {
                return (from row in data.AsEnumerable()
                        select new Table(
                            (int)row["TableObjectId"],
                            new Schema((string)row["SchemaName"]),
                            (string)row["TableName"],
                            null)
                    ).ToList<Table>();
            }
        }
    }
}
