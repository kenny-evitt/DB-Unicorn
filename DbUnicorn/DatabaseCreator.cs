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
        
        public void CreateSchemas()
        {
            string schemaScriptsFolderPath = Path.Combine(_scriptsRootFolderPath, "Schemas");

            ScriptExecutor.ExecuteScriptsInFolder(schemaScriptsFolderPath, _targetDatabase);
        }

        public void CreateTables()
        {
            string tableScriptsFolderPath = Path.Combine(_scriptsRootFolderPath, "Tables");

            ScriptExecutor.ExecuteScriptsInFolder(tableScriptsFolderPath, _targetDatabase);
        }

        public void CreateUserDefinedDataTypes()
        {
            string userDefinedDataTypeScriptsFolderPath = Path.Combine(_scriptsRootFolderPath, "User Defined Data Types");

            ScriptExecutor.ExecuteScriptsInFolder(userDefinedDataTypeScriptsFolderPath, _targetDatabase);
        }
    }
}
