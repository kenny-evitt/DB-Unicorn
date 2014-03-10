namespace DbUnicorn
{
    using System.Collections.Generic;

    public interface ISqlBatch
    {
        string Sql
        {
            get;
        }

        List<SqlBatchExecution> Executions
        {
            get;
            set;
        }
    }
}
