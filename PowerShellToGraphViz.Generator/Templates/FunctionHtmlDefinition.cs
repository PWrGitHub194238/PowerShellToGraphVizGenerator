using PowerShellToGraphViz.Json;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PowerShellToGraphVizGenerator.Templates
{
    public class FunctionHtmlDefinition
    {
        private const string FUNCTION_PARAMETER_DEFS = "{{function_parameter_html_definitions}}";

        public static readonly string Template = File.ReadAllText(
            Path.Combine(Program.basePath, Program.TEMPLATES_DIR, "FunctionHtmlDefinition.template"));

        private readonly string _functionScope;
        private readonly string _functionName;
        private readonly IList<FunctionParameterHtmlDefinition> _functionParameterHtmlDefinitions;

        public FunctionHtmlDefinition(Function functionDescriptionNode)
        {
            _functionScope = functionDescriptionNode.Scope;
            _functionName = functionDescriptionNode.Name;

            _functionParameterHtmlDefinitions = new List<FunctionParameterHtmlDefinition>();
            foreach (var functionParameterHtmlDefinition in functionDescriptionNode.Parameters)
            {
                _functionParameterHtmlDefinitions.Add(
                    new FunctionParameterHtmlDefinition(functionParameterHtmlDefinition));
            }
        }

        public string FillTemplate()
        {
            string filledTemplate = Template;

            filledTemplate = filledTemplate.Replace(TemplateTags.FUNCTION_SCOPE, _functionScope);
            filledTemplate = filledTemplate.Replace(TemplateTags.FUNCTION_NAME, _functionName);

            StringBuilder sb = new StringBuilder();

            foreach (var functionParameterHtmlDefinition in _functionParameterHtmlDefinitions)
            {
                sb.AppendLine(functionParameterHtmlDefinition.FillTemplate());
            }
            filledTemplate = filledTemplate.Replace(FUNCTION_PARAMETER_DEFS, sb.ToString());

            return filledTemplate;
        }
    }
}
