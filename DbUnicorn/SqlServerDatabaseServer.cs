namespace DbUnicorn
{
    using System;

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
            _masterDatabase.ExecuteSqlBatch(
                new SqlServerBatch(
                    String.Format(
                        @"CREATE DATABASE {0};",
                        TransactSqlHelpers.Identifiers.ValidIdentifier(databaseName))));

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
