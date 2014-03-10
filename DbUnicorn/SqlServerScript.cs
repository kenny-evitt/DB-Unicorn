namespace DbUnicorn
{
    using System.Collections.Generic;
    using System.Linq;

    using TransactSqlHelpers;

    public class SqlServerScript
    {
        private Script _transactSqlScript;
        private List<SqlServerBatch> _batches;

        public List<SqlServerBatch> Batches
        {
            get
            {
                return _batches;
            }
        }

        public SqlServerScript(string scriptPath)
        {
            _transactSqlScript = new Script(scriptPath);
            
            _batches =
                (from Batch batch in _transactSqlScript.Batches
                 select new SqlServerBatch(batch.Sql))
                 .ToList();
        }
    }
}
