namespace DbUnicorn.DatabaseObjects
{
    using System.Collections.Generic;

    public class Table
    {
        // Private fields
        
        private readonly List<TableColumn> _columns;
        private readonly string _name;
        private readonly int _objectId;
        private readonly Schema _schema;

        
        // Public properties
        
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

        public int ObjectId
        {
            get
            {
                return _objectId;
            }
        }

        public Schema Schema
        {
            get
            {
                return _schema;
            }
        }


        // Constructors

        public Table(int objectId, Schema schema, string name, List<TableColumn> columns)
        {
            _objectId = objectId;
            _schema = schema;
            _name = name;
            _columns = columns;
        }
    }
}
