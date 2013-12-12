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

        public List<StoredProcedure> GetStoredProcedures()
        {
            List<StoredProcedure> storedProcedures = new List<StoredProcedure>();

            using (SqlConnection dbConnection = new SqlConnection(_connectionString))
            using (SqlCommand dbSelectCommand = new SqlCommand(@"SELECT	SchemaName = s.name,
		ProcedureName = p.name,
		ProcedureText = m.[definition]
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
                                new Schema((string)reader["SchemaName"]),
                                (string)reader["ProcedureName"],
                                (string)reader["ProcedureText"]));
                    }
                }

                dbConnection.Close();
            }

            return storedProcedures;
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
