﻿namespace DbUnicorn
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using TransactSqlHelpers;

    public static class ScriptExecutor
    {
        public static void ExecuteScriptsInFolder(string folderPath, IDatabase targetDatabase)
        {
            foreach (string fileName in Directory.EnumerateFiles(folderPath, "*.sql"))
            {
                foreach (string batch in ScriptExecutor.GetScriptBatches(Path.Combine(folderPath, fileName)))
                {
                    targetDatabase.ExecuteSqlBatch(batch);
                }
            }
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
