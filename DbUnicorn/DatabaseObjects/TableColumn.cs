namespace DbUnicorn.DatabaseObjects
{
    using System.Data;
    using System.Data.SqlClient;

    public class TableColumn
    {
        private readonly string _name;
        private readonly SqlDbType _type;

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public SqlDbType Type
        {

            get
            {
                return _type;
            }
        }
    }
}
