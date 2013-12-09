namespace DbUnicorn.DatabaseObjects
{
    public class Schema
    {
        private readonly string _name;

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public Schema(string name)
        {
            _name = name;
        }
    }
}
