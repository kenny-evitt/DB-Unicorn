namespace DbUnicorn
{
    using System;
    using System.Linq;

    public class SqlServerDatabaseServer : IDatabaseServer
    {
        private const string _connectingStringFormat = @"Data Source={0};Database={1};Trusted_Connection=True;";
        
        private readonly SqlServerDatabase _masterDatabase;
        private readonly string _serverInstanceName;
        
        public SqlServerDatabaseServer(string serverInstanceName)
        {
            _serverInstanceName = serverInstanceName;
            
            _masterDatabase = new SqlServerDatabase(this.ConnectionString("master"));
        }

        public IDatabase CreateDatabase(string databaseName)
        {
            SqlServerBatch batch =
                new SqlServerBatch(
                    String.Format(
                        @"CREATE DATABASE {0};",
                        TransactSqlHelpers.Identifiers.ValidIdentifier(databaseName)));

            _masterDatabase.ExecuteSqlBatch(batch);

            Exception lastBatchExecutionException = batch.Executions.Last().Exception;

            if (lastBatchExecutionException != null)
                throw new ApplicationException("Database creation failed.", lastBatchExecutionException);

            return new SqlServerDatabase(this.ConnectionString(databaseName));
        }

        private string ConnectionString(string databaseName)
        {
            return
                String.Format(
                    _connectingStringFormat,
                    _serverInstanceName,
                    databaseName);
        }
    }
}
