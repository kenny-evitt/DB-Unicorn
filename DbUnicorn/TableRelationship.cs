namespace DbUnicorn
{
    using System;
    
    using DatabaseObjects;

    public class TableRelationship : IEquatable<TableRelationship>
    {
        private readonly int _level;
        
        private readonly Table _baseTable;
        private readonly Table _referencedTable;

        public int Level
        {
            get
            {
                return _level;
            }
        }

        public Table BaseTable
        {
            get
            {
                return _baseTable;
            }
        }

        public Table ReferencedTable
        {
            get
            {
                return _referencedTable;
            }
        }


        // Constructors

        public TableRelationship(int level, Table baseTable, Table referencedTable)
        {
            _level = level;
            _baseTable = baseTable;
            _referencedTable = referencedTable;
        }


        // Public methods

        public override bool Equals(object obj)
        {
            return Equals(obj as TableRelationship);
        }

        public bool Equals(TableRelationship other)
        {
            return _level == other.Level && _baseTable == other.BaseTable && _referencedTable == other.ReferencedTable;
        }

        public override int GetHashCode()
        {
            return _level.GetHashCode() ^ _baseTable.GetHashCode() ^ _referencedTable.GetHashCode();
        }
    }
}
