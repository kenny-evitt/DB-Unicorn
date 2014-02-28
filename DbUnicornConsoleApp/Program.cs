namespace DbUnicornConsoleApp
{
    using System;
    using System.Collections.Generic;

    using DbUnicorn;
    
    class Program
    {
        static void Main(string[] args)
        {
            string command = args[0];

            switch (command)
            {
                case "create-database":
                    CreateDatabase(args[1], args[2], args[3]);
                    break;

                default:
                    throw new ApplicationException(
                        String.Format("Command '{0}' is not recognized.", command));
            }
        }

        private static void CreateDatabase(string serverInstanceName, string databaseName, string scriptsRootFolderPath)
        {
            DatabaseCreator dbCreator =
                new DatabaseCreator(
                    scriptsRootFolderPath,
                    new SqlServerDatabaseServer(serverInstanceName));

            IDatabase targetDatabase = dbCreator.CreateDatabase(databaseName);

            dbCreator.CreateSchemas();
            dbCreator.CreateUserDefinedDataTypes();
            dbCreator.CreateTables();
        }
    }
}
