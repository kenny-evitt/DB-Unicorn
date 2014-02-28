namespace DbUnicorn
{
    using System.IO;

    using TransactSqlHelpers;

    public static class SqlServerScriptExecutor
    {
        public static void ExecuteScriptsInFolder(string folderPath, SqlServerDatabase targetDatabase)
        {
            foreach (string fileName in Directory.EnumerateFiles(folderPath, "*.sql"))
            {
                Script script = new Script(Path.Combine(folderPath, fileName));

                foreach (Batch batch in script.Batches)
                {
                    targetDatabase.ExecuteSqlBatch(batch.Sql);
                }
            }
        }
    }
}
