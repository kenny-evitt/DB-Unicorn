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


        // Operators

        public static bool operator ==(TableRelationship tableRelationship1, TableRelationship tableRelationship2)
        {
            if (System.Object.ReferenceEquals(tableRelationship1, tableRelationship2))
                return true;

            if (((object)tableRelationship1 == null) || ((object)tableRelationship2 == null))
                return false;

            return
                tableRelationship1._level == tableRelationship2._level
                && tableRelationship1._baseTable == tableRelationship2._baseTable
                && tableRelationship1._referencedTable == tableRelationship2._referencedTable;
        }

        public static bool operator !=(TableRelationship tableRelationship1, TableRelationship tableRelationship2)
        {
            return !(tableRelationship1 == tableRelationship2);
        }


        // Public methods

        public override bool Equals(object obj)
        {
            return Equals(obj as TableRelationship);
        }

        public bool Equals(TableRelationship other)
        {
            if ((object)other == null)
                return false;
            
            return _level == other.Level && _baseTable.Equals(other._baseTable) && _referencedTable.Equals(other._referencedTable);
        }

        public override int GetHashCode()
        {
            return _level.GetHashCode() ^ _baseTable.GetHashCode() ^ _referencedTable.GetHashCode();
        }
    }
}
