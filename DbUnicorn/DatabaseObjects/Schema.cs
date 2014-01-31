namespace DbUnicorn.DatabaseObjects
{
    using System;
    using System.Collections.Generic;

    public class Schema : IDatabaseObject, IEquatable<Schema>
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


        // Operators

        public static bool operator ==(Schema schema1, Schema schema2)
        {
            if (System.Object.ReferenceEquals(schema1, schema2))
                return true;

            if (((object)schema1 == null) || ((object)schema2 == null))
                return false;

            return schema1._name == schema2._name;
        }

        public static bool operator !=(Schema schema1, Schema schema2)
        {
            return !(schema1 == schema2);
        }
        
        
        // Public methods

        public override bool Equals(object obj)
        {
            return Equals(obj as Schema);
        }
        
        public bool Equals(Schema other)
        {
            if ((object)other == null)
                return false;
            
            return _name == other._name;
        }
        
        public static IEqualityComparer<Schema> GetEqualityComparer()
        {
            return new SchemaEqualityComparer();
        }

        public override int GetHashCode()
        {
            return _name.GetHashCode();
        }


        // Private nested classes

        private class SchemaEqualityComparer : IEqualityComparer<Schema>
        {
            public bool Equals(Schema schema1, Schema schema2)
            {
                return schema1.Equals(schema2);
            }

            public int GetHashCode(Schema schema)
            {
                return schema.GetHashCode();
            }
        }
    }
}
