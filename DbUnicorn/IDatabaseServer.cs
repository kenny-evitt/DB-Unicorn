namespace DbUnicorn
{
    public interface IDatabaseServer
    {
        IDatabase CreateDatabase(string databaseName);
    }
}
