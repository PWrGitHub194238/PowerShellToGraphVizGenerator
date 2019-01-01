using PowerShellToGraphViz.Json;
using System.Collections.Generic;
using System.IO;

namespace PowerShellToGraphVizGenerator.Templates
{
    public class FunctionHtmlDefinition : DotTemplateItem
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

        public override string FillTemplate()
        {
            string filledTemplate = Template;

            filledTemplate = filledTemplate.Replace(TemplateTags.FUNCTION_SCOPE, _functionScope);
            filledTemplate = filledTemplate.Replace(TemplateTags.FUNCTION_NAME, _functionName);

            filledTemplate = FillTemplateUtils.FillByList(
                inputString: filledTemplate,
                templateItemList: _functionParameterHtmlDefinitions,
                templatePlaceholderName: FUNCTION_PARAMETER_DEFS);

            return filledTemplate;
        }
    }
}
