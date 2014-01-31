namespace Tests
{
    using NUnit.Framework;

    using DbUnicorn;
    using DbUnicorn.DatabaseObjects;
    using Tests.TestDatabases;

    [TestFixture]
    public class TestDatabaseTester
    {
        [Test]
        public void ForeignKeyRelationshipTreeDatabaseRootTableHasBackwardReference()
        {
            ForeignKeyRelationshipTreeDatabase database = new ForeignKeyRelationshipTreeDatabase();
            DatabaseSchemaAndData databaseSchemaAndData = new DatabaseSchemaAndData(database);

            Assert.AreEqual(1, databaseSchemaAndData.GetTableForeignKeyRelationshipReferencers(database.RootTable).Count);
        }

        [Test]
        public void ForeignKeyRelationshipTreeDatabaseRootTableBackwardReferenceIsBackwardReferenceTable()
        {
            ForeignKeyRelationshipTreeDatabase database = new ForeignKeyRelationshipTreeDatabase();
            DatabaseSchemaAndData databaseSchemaAndData = new DatabaseSchemaAndData(database);

            Assert.AreEqual(
                database.BackwardReferenceTable.Name,
                databaseSchemaAndData.GetTableForeignKeyRelationshipReferencers(database.RootTable)[0].BaseTable.Name);

            Assert.AreEqual(
                database.BackwardReferenceTable,
                databaseSchemaAndData.GetTableForeignKeyRelationshipReferencers(database.RootTable)[0].BaseTable);
        }
    }
}
