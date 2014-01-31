namespace Tests.TestDatabases
{
    using System;
    using System.Data;

    public static class DatabaseHelper
    {
        public static DataTable GetTableRelationshipDataTable()
        {
            DataTable table = new DataTable();

            table.Columns.Add(
                new DataColumn()
                {
                    DataType = Type.GetType("System.Int32"),
                    ColumnName = "ObjectId"
                });

            table.Columns.Add(
                new DataColumn()
                {
                    DataType = Type.GetType("System.String"),
                    ColumnName = "SchemaName"
                });

            table.Columns.Add(
                new DataColumn()
                {
                    DataType = Type.GetType("System.String"),
                    ColumnName = "ObjectName"
                });

            return table;
        }
    }
}
