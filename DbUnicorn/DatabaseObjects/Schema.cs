namespace DbUnicorn.DatabaseObjects
{
    using System.Collections.Generic;

    public class Schema
    {
        // Private fields
        
        private readonly string _name;


        // Public properties

        public string Name
        {
            get
            {
                return _name;
            }
        }


        // Constructors

        public Schema(string name)
        {
            _name = name;
        }


        // Public methods

        public static IEqualityComparer<Schema> GetEqualityComparer()
        {
            return new SchemaEqualityComparer();
        }


        // Private nested classes

        private class SchemaEqualityComparer : IEqualityComparer<Schema>
        {
            public bool Equals(Schema schema1, Schema schema2)
            {
                return schema1.Name == schema2.Name;
            }

            public int GetHashCode(Schema schema)
            {
                return schema.Name.GetHashCode();
            }
        }
    }
}
