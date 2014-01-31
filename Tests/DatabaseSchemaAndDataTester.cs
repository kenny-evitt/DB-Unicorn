namespace Tests
{
    using System;
    using System.Data;
    
    using NUnit.Framework;

    using DbUnicorn;
    using DbUnicorn.DatabaseObjects;
    using Tests.TestDatabases;

    [TestFixture]
    public class DatabaseSchemaAndDataTester
    {
        [Test]
        public void SelfReferencingTableForeignKeyRelationshipContainsCorrectSelfReferences()
        {
            SelfReferencingTableDatabase database = new SelfReferencingTableDatabase();
            DatabaseSchemaAndData databaseSchemaAndData = new DatabaseSchemaAndData(database);

            Assert.AreEqual(1, databaseSchemaAndData.GenerateTableForeignKeyRelationshipTree(database.Table).Count);
        }

        [Test]
        public void TableForeignKeyRelationshipTreeIncludesBackwardsReferences()
        {
            ForeignKeyRelationshipTreeDatabase database = new ForeignKeyRelationshipTreeDatabase();
            DatabaseSchemaAndData databaseSchemaAndData = new DatabaseSchemaAndData(database);

            Assert.Contains(
                new TableRelationship(-1, database.BackwardReferenceTable, database.RootTable),
                databaseSchemaAndData.GenerateTableForeignKeyRelationshipTree(database.RootTable));
        }

        [Test]
        public void TableForeignKeyRelationshipReferencersAreCreatedCorrectly()
        {
            ForeignKeyRelationshipTreeDatabase database = new ForeignKeyRelationshipTreeDatabase();
            DatabaseSchemaAndData databaseSchemaAndData = new DatabaseSchemaAndData(database);

            Assert.AreEqual(
                database.BackwardReferenceTable.Name,
                databaseSchemaAndData.GetTableForeignKeyRelationshipReferencers(database.RootTable)[0].BaseTable.Name);

            Assert.AreEqual(
                database.BackwardReferenceTable,
                databaseSchemaAndData.GetTableForeignKeyRelationshipReferencers(database.RootTable)[0].BaseTable);

            Assert.AreEqual(
                database.RootTable,
                databaseSchemaAndData.GetTableForeignKeyRelationshipReferencers(database.RootTable)[0].ReferencedTable);
        }
    }
}
