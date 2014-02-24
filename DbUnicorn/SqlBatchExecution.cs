namespace DbUnicorn
{
    using System.Data.SqlClient;

    public class SqlBatchExecution
    {
        private readonly string _sqlBatch;

        public SqlException SqlException { get; set; }

        public SqlBatchExecution(string sqlBatch)
        {
            _sqlBatch = sqlBatch;
        }
    }
}
