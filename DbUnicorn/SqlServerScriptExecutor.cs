namespace DbUnicorn
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using TransactSqlHelpers;

    public static class SqlServerScriptExecutor
    {
        public static List<SqlServerScript> ExecuteScriptsInFolder(string folderPath, SqlServerDatabase targetDatabase, int? maxRetries)
        {
            return
                ExecuteScriptsInFolders(
                    new List<string> { folderPath },
                    targetDatabase,
                    maxRetries);
        }

        public static List<SqlServerScript> ExecuteScriptsInFolders(IEnumerable<string> folderPaths, SqlServerDatabase targetDatabase, int? maxRetries)
        {
            List<SqlServerScript> scripts = new List<SqlServerScript>();

            foreach (string folderPath in folderPaths)
            {
                foreach (string fileName in Directory.EnumerateFiles(folderPath, "*.sql"))
                {
                    SqlServerScript script = new SqlServerScript(Path.Combine(folderPath, fileName));

                    foreach (ISqlBatch batch in script.Batches)
                    {
                        targetDatabase.ExecuteSqlBatch(batch);
                    }

                    scripts.Add(script);
                }
            }

            // TODO: Implement unlimited retries (carefully).

            while (maxRetries > 0)
            {
                foreach (SqlServerScript script in scripts)
                {
                    foreach (SqlServerBatch batch in script.Batches)
                    {
                        if (batch.Executions.Last().Exception != null)
                            targetDatabase.ExecuteSqlBatch(batch);
                    }
                }

                maxRetries--;
            }

            return scripts;
        }
    }
}
