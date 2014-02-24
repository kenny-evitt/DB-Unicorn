namespace DbUnicorn
{
    using System.Data.SqlClient;

    public class SqlBatchExecution
    {
        private readonly string _sqlBatch;


        // Properties

        public string SqlBatch
        {
            get
            {
                return _sqlBatch;
            }
        }

        public SqlException SqlException { get; set; }


        // Constructor

        public SqlBatchExecution(string sqlBatch)
        {
            _sqlBatch = sqlBatch;
        }
    }
}
