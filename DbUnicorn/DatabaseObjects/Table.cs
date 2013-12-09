namespace DbUnicorn.DatabaseObjects
{
    using System.Collections.Generic;

    public class Table
    {
        private readonly List<TableColumn> _columns;
        private readonly string _name;
        private readonly Schema _schema;

        public List<TableColumn> Columns
        {
            get
            {
                return _columns;
            }
        }
        
        public string Name
        {
            get
            {
                return _name;
            }
        }

        public Schema Schema
        {
            get
            {
                return _schema;
            }
        }

        public Table(Schema schema, string name, List<TableColumn> columns)
        {
            _schema = schema;
            _name = name;
            _columns = columns;
        }
    }
}
