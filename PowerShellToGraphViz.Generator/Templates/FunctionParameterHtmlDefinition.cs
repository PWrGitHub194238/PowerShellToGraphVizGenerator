using PowerShellToGraphViz.Json;
using System.IO;

namespace PowerShellToGraphVizGenerator.Templates
{
    public class FunctionParameterHtmlDefinition : DotTemplateItem
    {
        public static readonly string Template = File.ReadAllText(
            Path.Combine(Program.basePath, Program.TEMPLATES_DIR, "FunctionParameterHtmlDefinition.template"));

        private readonly bool _isRequired;
        private readonly string _type;
        private readonly string _name;

        public FunctionParameterHtmlDefinition(Parameter functionParameterHtmlDefinition) :
            this(functionParameterHtmlDefinition.Reqiuired, functionParameterHtmlDefinition.Type, functionParameterHtmlDefinition.Name)
        {
        }

        public FunctionParameterHtmlDefinition(bool isRequired, string type, string name)
        {
            _isRequired = isRequired;
            _type = type;
            _name = name;
        }

        public override string FillTemplate()
        {
            string filledTemplate = Template;

            filledTemplate = filledTemplate.Replace(TemplateTags.FUNCTION_PARAM_IS_REQUIRED, _isRequired ? "T" : "N");
            filledTemplate = filledTemplate.Replace(TemplateTags.FUNCTION_PARAM_TYPE, _type);
            filledTemplate = filledTemplate.Replace(TemplateTags.FUNCTION_PARAM_NAME, _name);

            return filledTemplate;
        }
    }
}
