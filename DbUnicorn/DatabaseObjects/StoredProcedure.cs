namespace DbUnicorn.DatabaseObjects
{
    public class StoredProcedure
    {
        private readonly string _name;
        private readonly Schema _schema;
        private readonly string _text;

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

        public string Text
        {
            get
            {
                return _text;
            }
        }

        public StoredProcedure(Schema schema, string name, string text)
        {
            _schema = schema;
            _name = name;
            _text = text;
        }
    }
}
