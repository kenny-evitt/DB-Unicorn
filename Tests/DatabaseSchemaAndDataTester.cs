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
    }
}
