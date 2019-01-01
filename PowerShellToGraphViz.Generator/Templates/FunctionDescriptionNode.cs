using PowerShellToGraphViz.Json;
using System.IO;

namespace PowerShellToGraphVizGenerator.Templates
{
    public class FunctionDescriptionNode
    {
        public static readonly string Template = File.ReadAllText(
            Path.Combine(Program.basePath, Program.TEMPLATES_DIR, "FunctionDescriptionNode.template"));

        private readonly string _powerShellFileName;
        private readonly string _functionName;
        private readonly string _functionDescription;

        public FunctionDescriptionNode(string powerShellFileName, Function functionDescriptionNode) :
            this(powerShellFileName, functionDescriptionNode.Name, functionDescriptionNode.Description)
        {
        }

        public FunctionDescriptionNode(string powerShellFileName, string functionName, string functionDescription)
        {
            _powerShellFileName = powerShellFileName;
            _functionName = functionName;
            _functionDescription = functionDescription;
        }

        public string FillTemplate()
        {
            string filledTemplate = Template;

            filledTemplate = filledTemplate.Replace(TemplateTags.PS_FILE_NAME, _powerShellFileName);
            filledTemplate = filledTemplate.Replace(TemplateTags.FUNCTION_NAME, _functionName);
            filledTemplate = filledTemplate.Replace(TemplateTags.FUNCTION_DESCRIPTION, _functionDescription);

            return filledTemplate;
        }
    }
}
