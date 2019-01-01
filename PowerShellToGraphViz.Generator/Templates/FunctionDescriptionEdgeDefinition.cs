using System.IO;

namespace PowerShellToGraphVizGenerator.Templates
{
    public class FunctionDescriptionEdgeDefinition
    {
        public static readonly string Template = File.ReadAllText(
            Path.Combine(Program.basePath, Program.TEMPLATES_DIR, "FunctionDescriptionEdgeDefinition.template"));

        private readonly string _powerShellFileName;
        private readonly string _functionName;

        public FunctionDescriptionEdgeDefinition(string powerShellFileName, string functionName)
        {
            _powerShellFileName = powerShellFileName;
            _functionName = functionName;
        }

        public string FillTemplate()
        {
            string filledTemplate = Template;

            filledTemplate = filledTemplate.Replace(TemplateTags.PS_FILE_NAME, _powerShellFileName);
            filledTemplate = filledTemplate.Replace(TemplateTags.FUNCTION_NAME, _functionName);

            return filledTemplate;
        }
    }
}
