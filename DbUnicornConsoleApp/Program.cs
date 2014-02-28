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


        // Private methods

        private static void CreateDatabase(string serverInstanceName, string databaseName, string scriptsRootFolderPath)
        {
            IDatabaseServer server = Program.GetServer("sql-server", serverInstanceName);

            IDatabase targetDatabase = server.CreateDatabase(databaseName);

            targetDatabase.CreateObjectsFromScripts(scriptsRootFolderPath);
        }

        private static IDatabaseServer GetServer(string serverType, string serverInstanceName)
        {
            IDatabaseServer server;

            switch (serverType)
            {
                case "sql-server":
                    server = new SqlServerDatabaseServer(serverInstanceName);
                    break;

                default:
                    throw new ArgumentException(
                        String.Format("Server type '{0}' is not supported.", serverType));
            }

            return server;
        }
    }
}
