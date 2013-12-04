namespace DbUnicorn.DatabaseObjects
{
    public class StoredProcedure
    {
        private readonly string _name;
        private readonly string _schemaName;
        private readonly string _text;

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public string SchemaName
        {
            get
            {
                return _schemaName;
            }
        }

        public string Text
        {
            get
            {
                return _text;
            }
        }

        public StoredProcedure(string schemaName, string name, string text)
        {
            _schemaName = schemaName;
            _name = name;
            _text = text;
        }
    }
}
