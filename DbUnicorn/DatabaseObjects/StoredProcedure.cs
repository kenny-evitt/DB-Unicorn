namespace DbUnicorn.DatabaseObjects
{
    using System;

    public class StoredProcedure : IDatabaseObject
    {
        private readonly string _name;
        private readonly Schema _schema;
        private readonly string _text;
        private readonly bool _usesAnsiNulls;
        private readonly bool _usesQuotedIdentifiers;

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

        public StoredProcedure(Schema schema, string name, string text, bool usesAnsiNulls, bool usesQuotedIdentifiers)
        {
            _schema = schema;
            _name = name;
            _text = text;
            _usesAnsiNulls = usesAnsiNulls;
            _usesQuotedIdentifiers = usesQuotedIdentifiers;
        }


        // Public methods

        public string GenerateDropAndCreateScript()
        {
            string compositeFormatScriptTemplate = @"IF EXISTS ( SELECT *
			FROM sys.objects
			WHERE	[object_id] = OBJECT_ID(N'{0}')
					AND [type] IN ( N'P', N'PC' ) )

	DROP PROCEDURE {0};
GO

SET ANSI_NULLS {1};
GO

SET QUOTED_IDENTIFIER {2};
GO

{3}
GO";

            return GenerateDropAndCreateScript(compositeFormatScriptTemplate);
        }

        public string GenerateDropAndCreateScript(string scriptTemplate)
        {
            return GenerateDropAndCreateScript(scriptTemplate, "{0}.{1}");
        }

        public string GenerateDropAndCreateScript(string scriptTemplate, string procedureNameTemplate)
        {
            return GenerateDropAndCreateScript(scriptTemplate, procedureNameTemplate, false);
        }

        public string GenerateDropAndCreateScript(string scriptTemplate, string procedureNameTemplate, bool replaceProcedureNameInProcedureText)
        {
            string procedureName = String.Format(procedureNameTemplate, _schema.Name, _name);
            
            return String.Format(
                    scriptTemplate,
                    procedureName,
                    ScriptingHelpers.ConvertBooleanToOnOrOffString(_usesAnsiNulls),
                    ScriptingHelpers.ConvertBooleanToOnOrOffString(_usesQuotedIdentifiers),
                    !replaceProcedureNameInProcedureText ? _text : ReplaceProcedureNameInProcedureText(procedureName));
        }


        // Private methods

        private string ReplaceProcedureNameInProcedureText(string newProcedureName)
        {
            Tuple<int, int> procedureNameStartAndLength = ScriptingHelpers.FindStoredProcedureNameInCreateScript(_text);

            int procedureNameStart = procedureNameStartAndLength.Item1;

            return _text.Remove(procedureNameStart, procedureNameStartAndLength.Item2)
                .Insert(procedureNameStart, newProcedureName);
        }
    }
}
