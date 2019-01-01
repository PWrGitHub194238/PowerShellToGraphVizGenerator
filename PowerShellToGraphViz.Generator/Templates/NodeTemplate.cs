using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PowerShellToGraphVizGenerator.Templates
{
    public class NodeTemplate : DotTemplateItem
    {
        private const string FUNCTION_DESCRIPTION_NODES = "{{funtion_description_nodes}}";
        private const string FUNCTION_HTML_DEFS = "{{function_html_definitions}}";
        private const string FUNCTION_DESCRIPTION_EDGE_DEFS = "{{function_description_edge_definitions}}";

        public static readonly string Template = File.ReadAllText(
            Path.Combine(Program.basePath, Program.TEMPLATES_DIR, "NodeTemplate.template"));

        private readonly string _powerShellFileName;
        private readonly IList<FunctionDescriptionNode> _functionDescriptionNodes;
        private readonly IList<FunctionHtmlDefinition> _functionHtmlDefinitions;
        private readonly IList<FunctionDescriptionEdgeDefinition> _functionDescriptionEdgeDefinitions;

        public NodeTemplate(PowerShellToGraphViz.Json.NodeTemplate json)
        {
            _powerShellFileName = json.FileName;

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

        public override string FillTemplate()
        {
            string filledTemplate = Template;

            filledTemplate = filledTemplate.Replace(TemplateTags.PS_FILE_NAME, _powerShellFileName);

            filledTemplate = FillFunctionDescriptions(filledTemplate, _functionDescriptionNodes);
            filledTemplate = FillFunctionDefinitions(filledTemplate, _functionHtmlDefinitions);
            filledTemplate = FillEdgeDefinitions(filledTemplate, _functionDescriptionEdgeDefinitions);

            return filledTemplate;
        }

        private string FillFunctionDescriptions(string filledTemplate,
            IList<FunctionDescriptionNode> functionDescriptionNodes)
        {
            return FillTemplateUtils.FillByList(
                inputString: filledTemplate,
                templateItemList: functionDescriptionNodes,
                templatePlaceholderName: FUNCTION_DESCRIPTION_NODES);
        }

        private string FillFunctionDefinitions(string filledTemplate,
            IList<FunctionHtmlDefinition> functionHtmlDefinitions)
        {
            return FillTemplateUtils.FillByList(
                inputString: filledTemplate,
                templateItemList: functionHtmlDefinitions,
                templatePlaceholderName: FUNCTION_HTML_DEFS);
        }

        private string FillEdgeDefinitions(string filledTemplate,
            IList<FunctionDescriptionEdgeDefinition> functionDescriptionEdgeDefinitions)
        {
            return FillTemplateUtils.FillByList(
                inputString: filledTemplate,
                templateItemList: functionDescriptionEdgeDefinitions,
                templatePlaceholderName: FUNCTION_DESCRIPTION_EDGE_DEFS);
        }
    }
}
