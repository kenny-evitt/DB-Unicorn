namespace DbUnicorn
{
    using System.Collections.Generic;
    using System.IO;

    public static class ScriptExecutor
    {
        public static void ExecuteScriptsInFolder(string folderPath, IDatabase targetDatabase)
        {
            foreach (string fileName in Directory.EnumerateFiles(folderPath, "*.sql"))
            {
                foreach (string batch in ScriptExecutor.GetScriptBatches(Path.Combine(folderPath, fileName)))
                {
                    targetDatabase.ExecuteSql(batch);
                }
            }
        }

        private static IEnumerable<string> GetScriptBatches(string scriptFilePath)
        {
            return
                TransactSqlHelpers.Scripts.GetBatches(
                    File.ReadAllLines(scriptFilePath));
        }
    }
}
