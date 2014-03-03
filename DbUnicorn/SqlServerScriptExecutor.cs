namespace DbUnicorn
{
    using System.Collections.Generic;
    using System.IO;

    using TransactSqlHelpers;

    public static class SqlServerScriptExecutor
    {
        public static List<SqlServerScript> ExecuteScriptsInFolder(string folderPath, SqlServerDatabase targetDatabase)
        {
            List<SqlServerScript> scripts = new List<SqlServerScript>();
            
            foreach (string fileName in Directory.EnumerateFiles(folderPath, "*.sql"))
            {
                SqlServerScript script = new SqlServerScript(Path.Combine(folderPath, fileName));

                foreach (ISqlBatch batch in script.Batches)
                {
                    targetDatabase.ExecuteSqlBatch(batch);
                }

                scripts.Add(script);
            }

            return scripts;
        }
    }
}
