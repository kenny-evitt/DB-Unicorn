namespace DbUnicorn
{
    using System.Collections.Generic;
    using System.IO;

    public static class ScriptExecutor
    {
        public static IEnumerable<SqlBatchExecution> ExecuteScriptsInFolder(string folderPath, IDatabase targetDatabase)
        {
            List<SqlBatchExecution> scriptBatchExecutions = new List<SqlBatchExecution>();
            
            foreach (string fileName in Directory.EnumerateFiles(folderPath, "*.sql"))
            {
                foreach (string batch in ScriptExecutor.GetScriptBatches(Path.Combine(folderPath, fileName)))
                {
                    scriptBatchExecutions.Add(
                        targetDatabase.ExecuteSqlBatch(batch));
                }
            }

            return scriptBatchExecutions;
        }

        private static IEnumerable<string> GetScriptBatches(string scriptFilePath)
        {
            return
                TransactSqlHelpers.Scripts.GetBatches(
                    File.ReadAllLines(scriptFilePath));
        }
    }
}
