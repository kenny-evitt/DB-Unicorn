namespace DbUnicorn
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public class DatabaseCreator
    {
        private readonly string _scriptsRootFolderPath;
        private readonly IDatabase _targetDatabase;
        
        public DatabaseCreator(string scriptsRootFolderPath, IDatabase targetDatabase)
        {
            _scriptsRootFolderPath = scriptsRootFolderPath;
            _targetDatabase = targetDatabase;
        }


        // Public methods

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
