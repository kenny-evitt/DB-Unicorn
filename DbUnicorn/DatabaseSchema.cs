namespace DbUnicorn
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    using DatabaseObjects;

    public class DatabaseSchema
    {
        private readonly string _connectionString;
        
        public DatabaseSchema(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<StoredProcedure> GetStoredProcedures()
        {
            List<StoredProcedure> _storedProcedures = new List<StoredProcedure>();

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
                        _storedProcedures.Add(
                            new StoredProcedure(
                                (string)reader["SchemaName"],
                                (string)reader["ProcedureName"],
                                (string)reader["ProcedureText"]));
                    }
                }

                dbConnection.Close();
            }

            return _storedProcedures;
        }
    }
}
