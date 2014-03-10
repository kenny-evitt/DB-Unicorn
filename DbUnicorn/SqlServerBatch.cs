namespace DbUnicorn
{
    using System.Collections.Generic;

    public class SqlServerBatch : ISqlBatch
    {
        private readonly string _sql;
        
        public string Sql
        {
            get
            {
                return _sql;
            }
        }

        public List<SqlBatchExecution> Executions
        {
            get;
            set;
        }

        public SqlServerBatch(string sql)
        {
            _sql = sql;
            this.Executions = new List<SqlBatchExecution>();
        }
    }
}
