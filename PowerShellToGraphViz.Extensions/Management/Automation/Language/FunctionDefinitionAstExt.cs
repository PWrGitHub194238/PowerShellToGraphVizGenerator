using System.Management.Automation.Language;

namespace PowerShellToGraphViz.Extensions.Management.Automation.Language
{
    public static class FunctionDefinitionAstExt
    {
        public static string GetFunctionScopeName(this FunctionDefinitionAst functionDefinitionAst)
        {
            string functionFullName = functionDefinitionAst.Name;
            int pos = functionFullName.IndexOf(':');
            return functionFullName.Substring(0, pos < 0 ? 0 : pos);
        }

        public static string GetFunctionLocalName(this FunctionDefinitionAst functionDefinitionAst)
        {
            string functionFullName = functionDefinitionAst.Name;
            return functionFullName.Substring(functionFullName.IndexOf(':') + 1);
        }

        public static bool DependsOn(this FunctionDefinitionAst functionDefinitionAst,
            string dependencyFunctionName)
        {
            return functionDefinitionAst.Body.Extent.Text.Contains(dependencyFunctionName);
        }
    }
}
