namespace DbUnicorn
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public class DatabaseCreator
    {
        // Private fields

        private readonly string _scriptsRootFolderPath;
        private readonly IDatabaseServer _targetDatabaseServer;

        private IDatabase _targetDatabase;
        

        // Constructor

        public DatabaseCreator(string scriptsRootFolderPath, IDatabaseServer targetDatabaseServer)
        {
            _scriptsRootFolderPath = scriptsRootFolderPath;
            _targetDatabaseServer = targetDatabaseServer;
        }


        // Public methods

        public IDatabase CreateDatabase(string databaseName)
        {
            _targetDatabase = _targetDatabaseServer.CreateDatabase(databaseName);

            return _targetDatabase;
        }
        
        public IEnumerable<SqlBatchExecution> CreateSchemas()
        {
            string schemaScriptsFolderPath = Path.Combine(_scriptsRootFolderPath, "Schemas");

            return ScriptExecutor.ExecuteScriptsInFolder(schemaScriptsFolderPath, _targetDatabase);
        }

        public IEnumerable<SqlBatchExecution> CreateTables()
        {
            string tableScriptsFolderPath = Path.Combine(_scriptsRootFolderPath, "Tables");

            return ScriptExecutor.ExecuteScriptsInFolder(tableScriptsFolderPath, _targetDatabase);
        }

        public IEnumerable<SqlBatchExecution> CreateUserDefinedDataTypes()
        {
            string userDefinedDataTypeScriptsFolderPath = Path.Combine(_scriptsRootFolderPath, "User Defined Data Types");

            return ScriptExecutor.ExecuteScriptsInFolder(userDefinedDataTypeScriptsFolderPath, _targetDatabase);
        }
    }
}
