namespace DbUnicorn
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using TransactSqlHelpers;

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
            IEnumerable<Batch> batches =
                Scripts.GetBatches(
                    File.ReadAllLines(scriptFilePath));

            return
                (from Batch batch in batches
                 select batch.Sql);
        }
    }
}
