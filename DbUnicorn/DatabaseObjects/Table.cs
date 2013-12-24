namespace DbUnicorn.DatabaseObjects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class Table : IDatabaseObject, IEquatable<Table>
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


        // Public methods

        public override bool Equals(object obj)
        {
            return Equals(obj as Table);
        }

        public bool Equals(Table other)
        {
            return _schema == other.Schema && _name == other.Name && _objectId == other.ObjectId;
        }

        public override int GetHashCode()
        {
            return _schema.GetHashCode() ^ _name.GetHashCode() ^ _objectId.GetHashCode();
        }


        // Private methods

        /// <summary>
        /// Generates foreign-key table relationship info in the DOT language format. See 
        /// http://www.graphviz.org/content/dot-language for more info. The DOT language is used
        /// by Graphviz for generating diagrams from 'graphs'.
        /// </summary>
        /// <returns>A DOT language string</returns>
        private string GenerateForeignKeyRelationshipsDot(List<TableRelationship> relationshipTree)
        {
            StringBuilder relationshipsDotString = new StringBuilder();

            relationshipsDotString.AppendLine("digraph G {");

            relationshipsDotString.AppendLine(
                String.Format(
                    "  \"{0}.{1}\"[shape=box color=\"red\"];",
                    TransactSqlHelpers.Identifiers.ValidIdentifier(_schema.Name),
                    TransactSqlHelpers.Identifiers.ValidIdentifier(_name)));

            foreach (TableRelationship uniqueTableRelationship in relationshipTree.Distinct())
            {
                if (uniqueTableRelationship.Level > 0)
                {
                    relationshipsDotString.AppendLine(
                        String.Format(
                            "  \"{0}.{1}\"[shape=box];",
                            TransactSqlHelpers.Identifiers.ValidIdentifier(uniqueTableRelationship.ReferencedTable.Schema.Name),
                            TransactSqlHelpers.Identifiers.ValidIdentifier(uniqueTableRelationship.ReferencedTable.Name)));
                }
                else
                {
                    relationshipsDotString.AppendLine(
                        String.Format(
                            "  \"{0}.{1}\"[shape=box];",
                            TransactSqlHelpers.Identifiers.ValidIdentifier(uniqueTableRelationship.BaseTable.Schema.Name),
                            TransactSqlHelpers.Identifiers.ValidIdentifier(uniqueTableRelationship.BaseTable.Name)));
                }
            }

            foreach (TableRelationship reference in relationshipTree)
            {
                relationshipsDotString.AppendLine(
                    String.Format(
                        "  \"{0}.{1}\" -> \"{2}.{3}\";",
                        TransactSqlHelpers.Identifiers.ValidIdentifier(reference.BaseTable.Schema.Name),
                        TransactSqlHelpers.Identifiers.ValidIdentifier(reference.BaseTable.Name),
                        TransactSqlHelpers.Identifiers.ValidIdentifier(reference.ReferencedTable.Schema.Name),
                        TransactSqlHelpers.Identifiers.ValidIdentifier(reference.ReferencedTable.Name)));
            }

            relationshipsDotString.Append("}");
            
            return relationshipsDotString.ToString();
        }
    }
}
