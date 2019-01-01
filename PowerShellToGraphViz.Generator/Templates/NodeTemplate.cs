using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PowerShellToGraphVizGenerator.Templates
{
    public class NodeTemplate
    {
        private const string FUNCTION_DESCRIPTION_NODES = "{{funtion_description_nodes}}";
        private const string FUNCTION_HTML_DEFS = "{{function_html_definitions}}";
        private const string FUNCTION_DESCRIPTION_EDGE_DEFS = "{{function_description_edge_definitions}}";

        public static readonly string Template = File.ReadAllText(
            Path.Combine(Program.basePath, Program.TEMPLATES_DIR, "NodeTemplate.template"));

        private readonly string _powerShellFileName;
        private readonly string _powerShellFileDescription;
        private readonly IList<FunctionDescriptionNode> _functionDescriptionNodes;
        private readonly IList<FunctionHtmlDefinition> _functionHtmlDefinitions;
        private readonly IList<FunctionDescriptionEdgeDefinition> _functionDescriptionEdgeDefinitions;

        public NodeTemplate(PowerShellToGraphViz.Json.NodeTemplate json)
        {
            _powerShellFileName = json.FileName;
            _powerShellFileDescription = json.FileDescription;

            _functionDescriptionNodes = new List<FunctionDescriptionNode>();
            _functionHtmlDefinitions = new List<FunctionHtmlDefinition>();
            _functionDescriptionEdgeDefinitions = new List<FunctionDescriptionEdgeDefinition>();
            foreach (var functionDescriptionNode in json.Functions)
            {
                _functionDescriptionNodes.Add(
                    new FunctionDescriptionNode(json.FileName, functionDescriptionNode));

                _functionHtmlDefinitions.Add(
                    new FunctionHtmlDefinition(functionDescriptionNode));

                _functionDescriptionEdgeDefinitions.Add(
                    new FunctionDescriptionEdgeDefinition(json.FileName, functionDescriptionNode.Name));
            }
        }

        public string FillTemplate()
        {
            string filledTemplate = Template;

            filledTemplate = filledTemplate.Replace(TemplateTags.PS_FILE_NAME, _powerShellFileName);
            filledTemplate = filledTemplate.Replace(TemplateTags.PS_FILE_DESCRIPTION, _powerShellFileDescription);

            StringBuilder sb = new StringBuilder();

            foreach (var functionDescriptionNode in _functionDescriptionNodes)
            {
                sb.AppendLine(functionDescriptionNode.FillTemplate());
            }
            filledTemplate = filledTemplate.Replace(FUNCTION_DESCRIPTION_NODES, sb.ToString());

            sb = new StringBuilder();

            foreach (var functionHtmlDefinition in _functionHtmlDefinitions)
            {
                sb.AppendLine(functionHtmlDefinition.FillTemplate());
            }
            filledTemplate = filledTemplate.Replace(FUNCTION_HTML_DEFS, sb.ToString());

            sb = new StringBuilder();

            foreach (var functionDescriptionEdgeDefinition in _functionDescriptionEdgeDefinitions)
            {
                sb.AppendLine(functionDescriptionEdgeDefinition.FillTemplate());
            }
            filledTemplate = filledTemplate.Replace(FUNCTION_DESCRIPTION_EDGE_DEFS, sb.ToString());

            return filledTemplate;
        }
    }
}
