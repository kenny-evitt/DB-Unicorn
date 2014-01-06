namespace DbUnicorn.DatabaseObjects
{
    using System;
    using System.Text.RegularExpressions;

    public static class ScriptingHelpers
    {
        public static string ConvertBooleanToOnOrOffString(bool boolean)
        {
            return boolean ? "ON" : "OFF";
        }

        /// <summary>
        /// Find the stored procedure name in a string containing a Transact-SQL 'create script'.
        /// </summary>
        /// <param name="createScript"></param>
        /// <returns>A 2-tuple (pair) containing the index of the first character and the length of
        /// the stored procedure name in the 'create script' string.</returns>
        public static Tuple<int, int> FindStoredProcedureNameInCreateScript(string createScript)
        {
            Regex storedProcedureNameRegex = new Regex(@"CREATE\s+PROC(?:EDURE)?\s+(?<fullName>(?:(?<schemaName>[\p{L}_][\p{L}\p{N}@$#_]{0,127}|[""\[][^\[\]]{1,128}[""\]])\.)?(?<procedureName>[\p{L}_][\p{L}\p{N}@$#_]{0,127}|[""\[][^\[\]]{1,128}[""\]]))(?:\s+|\()", RegexOptions.IgnoreCase);

            Match match = storedProcedureNameRegex.Match(createScript);

            if (!match.Success)
                throw new ArgumentException("No CREATE PROCEDURE statement was found in the supplied create script string.");

            Group fullNameGroup = match.Groups["fullName"];

            return Tuple.Create<int, int>(fullNameGroup.Index, fullNameGroup.Length);
        }
    }
}
