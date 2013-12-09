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
            List<Schema> schemas = new List<Schema>();
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
                    string storedProcedureSchemaName = null;
                    
                    while (reader.Read())
                    {
                        storedProcedureSchemaName = (string)reader["SchemaName"];

                        Schema storedProcedureSchema = schemas.Find(s => s.Name == storedProcedureSchemaName);

                        if (storedProcedureSchema == null)
                        {
                            storedProcedureSchema = new Schema(storedProcedureSchemaName);
                            schemas.Add(storedProcedureSchema);
                        }

                        storedProcedures.Add(
                            new StoredProcedure(
                                storedProcedureSchema,
                                (string)reader["ProcedureName"],
                                (string)reader["ProcedureText"]));
                    }
                }

                dbConnection.Close();
            }

            return storedProcedures;
        }

        
    }
}
