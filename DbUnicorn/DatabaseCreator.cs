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
        
        public void CreateSchemas()
        {
            string schemaScriptsFolderPath = Path.Combine(_scriptsRootFolderPath, "Schemas");

            SqlServerScriptExecutor.ExecuteScriptsInFolder(schemaScriptsFolderPath, _targetDatabase);
        }

        public void CreateTables()
        {
            string tableScriptsFolderPath = Path.Combine(_scriptsRootFolderPath, "Tables");

            SqlServerScriptExecutor.ExecuteScriptsInFolder(tableScriptsFolderPath, _targetDatabase);
        }

        public void CreateUserDefinedDataTypes()
        {
            string userDefinedDataTypeScriptsFolderPath = Path.Combine(_scriptsRootFolderPath, "User Defined Data Types");

            SqlServerScriptExecutor.ExecuteScriptsInFolder(userDefinedDataTypeScriptsFolderPath, _targetDatabase);
        }
    }
}
