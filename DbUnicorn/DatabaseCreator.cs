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

        public void CreateTables()
        {
            string tableScriptsFolderPath = Path.Combine(_scriptsRootFolderPath, "Tables");

            foreach (string fileName in Directory.EnumerateFiles(tableScriptsFolderPath, "*.sql"))
            {
                foreach (string batch in this.GetScriptBatches(Path.Combine(tableScriptsFolderPath, fileName)))
                {
                    _targetDatabase.ExecuteSql(batch);
                }
            }
        }

        private IEnumerable<string> GetScriptBatches(string scriptFilePath)
        {
            string[] scriptLines = File.ReadAllLines(scriptFilePath);

            List<string> batches = new List<string>();

            StringBuilder nextBatch = new StringBuilder();

            foreach (string line in scriptLines)
            {
                if (line.Trim().StartsWith("GO", System.StringComparison.InvariantCultureIgnoreCase))
                {
                    batches.Add(nextBatch.ToString());
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
