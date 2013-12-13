namespace DbUnicorn.DatabaseObjects
{
    public static class ScriptingHelpers
    {
        public static string ConvertBooleanToOnOrOffString(bool boolean)
        {
            return boolean ? "ON" : "OFF";
        }
    }
}
