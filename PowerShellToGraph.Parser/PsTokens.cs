using System.Management.Automation.Language;

namespace PowerShellToGraph.Parser
{
    public static class PsTokens
    {
        public const string TRUE_LOWER_TOKEN = "$true";
        public const string SYNOPSIS_LOWER_TOKEN = ".synopsis";
        public const string IMPORT_FUNCTION_LOWER_TOKEN = "import-function";
        public const string PARAMETER_LOWER_TOKEN = "parameter";

        public const string PARAMETER_ATTR_MANDATORY_LOWER_TOKEN = "mandatory";
        public const string PARAMETER_ATTR_SWITCH_LOWER_TOKEN = "switch";

        public static string GetTokenFormat(string text)
        {
            return text.Trim().ToLower();
        }

        public static bool StartsWith(Ast text, string token)
        {
            return GetTokenFormat(text.Extent.Text).StartsWith(token);
        }

        public static bool StartsWith(string text, string token)
        {
            return GetTokenFormat(text).StartsWith(token);
        }

        public static bool IsEqual(Ast text, string token)
        {
            return GetTokenFormat(text.Extent.Text).Equals(token);
        }

        public static bool IsEqual(string text, string token)
        {
            return GetTokenFormat(text).Equals(token);
        }

        public static bool Contains(Ast text, string token)
        {
            return GetTokenFormat(text.Extent.Text).Contains(token);
        }

        public static bool Contains(string text, string token)
        {
            return GetTokenFormat(text).Contains(token);
        }
    }
}
