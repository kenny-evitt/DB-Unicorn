namespace DbUnicorn.DatabaseObjects
{
    using System;

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
            return String.Format(scriptTemplate, String.Format("{0}.{1}", _schema.Name, _name), "ON", "ON", _text);
        }
    }
}
