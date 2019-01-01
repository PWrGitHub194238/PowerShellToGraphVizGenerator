namespace PowerShellToGraphVizGenerator.Templates
{
    public static class TemplateTags
    {
        public const string PS_FILE_NAME = "{PS_file_name}";
        public const string FUNCTION_SCOPE = "{fun<FID>_scope}";
        public const string FUNCTION_NAME = "{fun<FID>_name}";
        public const string FUNCTION_DESCRIPTION = "{fun<FID>_desc}";
        public const string FUNCTION_PARAM_IS_REQUIRED = "{fun<FID>par<PID>_required}";
        public const string FUNCTION_PARAM_TYPE = "{fun<FID>par<PID>_type}";
        public const string FUNCTION_PARAM_NAME = "{fun<FID>par<PID>_name}";
    }
}
