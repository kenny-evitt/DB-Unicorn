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

            this.ExecuteScripts(schemaScriptsFolderPath);
        }

        public void CreateTables()
        {
            string tableScriptsFolderPath = Path.Combine(_scriptsRootFolderPath, "Tables");

            this.ExecuteScripts(tableScriptsFolderPath);
        }

        public void CreateUserDefinedDataTypes()
        {
            string userDefinedDataTypeScriptsFolderPath = Path.Combine(_scriptsRootFolderPath, "User Defined Data Types");

            this.ExecuteScripts(userDefinedDataTypeScriptsFolderPath);
        }


        // Private methods

        private void ExecuteScripts(string folderPath)
        {
            foreach (string fileName in Directory.EnumerateFiles(folderPath, "*.sql"))
            {
                foreach (string batch in this.GetScriptBatches(Path.Combine(folderPath, fileName)))
                {
                    _targetDatabase.ExecuteSql(batch);
                }
            }
        }

        private IEnumerable<string> GetScriptBatches(string scriptFilePath)
        {
            string[] scriptLines = File.ReadAllLines(scriptFilePath);

            // TODO: Replace the following code with the new method in the Transact-SQL Helpers
            // library.

            List<string> batches = new List<string>();

            StringBuilder nextBatch = new StringBuilder();

            foreach (string line in scriptLines)
            {
                if (line.Trim().StartsWith("GO", System.StringComparison.InvariantCultureIgnoreCase))
                {
                    batches.Add(nextBatch.ToString());
                    nextBatch.Clear();
                }
                else
                {
                    nextBatch.AppendLine(line);
                }
            }

            return batches;
        }
    }
}
