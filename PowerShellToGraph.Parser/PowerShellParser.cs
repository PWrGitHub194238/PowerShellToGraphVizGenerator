using PowerShellToGraphViz.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation.Language;
using System.Text.RegularExpressions;

namespace PowerShellToGraph.Parser
{
    public static class PowerShellParser
    {
        public static NodeTemplate Parse(string powerShellFilePath)
        {
            IDictionary<string, Function> functionDefinitions = new Dictionary<string, Function>();

            var ast = System.Management.Automation.Language.Parser.ParseFile(powerShellFilePath, out Token[] tokens, out ParseError[] errors);
            if (errors.Length != 0) return null;


            string[] synopsis = tokens.Where(t => t.Kind == TokenKind.Comment).Select(c =>
            {
                Regex.Match(c.Extent.Text, "");
                return "";
            }).ToArray();

            var statements = ast.EndBlock.Statements;

            var imports = statements.Where(s =>
            {
                bool result = true;
                result &= s.GetType().Equals(typeof(PipelineAst));
                result &= s.Extent.Text.TrimStart().StartsWith("Import-Function");
                return result;
            }).Select(s =>
            {
                string import = s.Extent.Text.TrimEnd();
                return import.Substring(import.LastIndexOf(' ') + 1);
            }).ToList();
            var functions = statements.Where(s => s.GetType().Equals(typeof(FunctionDefinitionAst))).Select(s => (FunctionDefinitionAst)s).ToList();

            int i = 0;
            foreach (FunctionDefinitionAst fun in functions)
            {
                var name = fun.Name;
                var scope = "global";

                if (name.Contains(':'))
                {
                    scope = name.Substring(0, name.IndexOf(':'));
                    name = name.Substring(name.IndexOf(':') + 1);
                }
                imports.Add(name);
                if (i < synopsis.Length)
                {
                    functionDefinitions.Add(name, new Function(scope, name, synopsis[i++]));
                }
                else
                {
                    functionDefinitions.Add(name, new Function(scope, name, "Brak opisu :("));
                }
            }

            foreach (FunctionDefinitionAst fun in functions)
            {

                IList<Parameter> parameterList = new List<Parameter>();
                var name = fun.Name;

                if (name.Contains(':'))
                {
                    name = name.Substring(name.IndexOf(':') + 1);
                }
                if (fun.Body.ParamBlock != null)
                {
                    var parameters = fun.Body.ParamBlock.Parameters;

                    foreach (ParameterAst parameter in parameters)
                    {
                        var paramIsReq = false;

                        AttributeAst parameterAttr = (AttributeAst)parameter.Attributes.Where(a => a.TypeName.Name.ToLower().Equals("parameter")).FirstOrDefault();
                        if (parameterAttr != null)
                        {
                            var val = parameterAttr.NamedArguments.Where(a => a.ArgumentName.ToLower().Equals("mandatory")).FirstOrDefault();
                            var str = val.Argument.ToString();
                            paramIsReq = val.Argument.ToString().Equals("$true");
                            var paramName = parameter.Name.Extent.Text;
                            var paramType = parameter.StaticType.Name;
                            parameterList.Add(new Parameter(paramIsReq, paramType, paramName));
                        }
                        TypeConstraintAst switchAttr = (TypeConstraintAst)parameter.Attributes.Where(a => a.TypeName.Name.ToLower().Equals("switch")).FirstOrDefault();
                        if (switchAttr != null)
                        {
                            parameterList.Add(new Parameter(false, "switch", parameter.Name.Extent.Text));
                        }
                    }
                }
                IList<Dependency> dependsOnList = new List<Dependency>();

                foreach (string import in imports)
                {
                    if (fun.Body.Extent.Text.Contains(import))
                    {
                        dependsOnList.Add(new Dependency(import));
                    }
                }

                functionDefinitions[name].Parameters = parameterList;
                functionDefinitions[name].DependsOn = dependsOnList;
            }


            return new NodeTemplate(Path.GetFileNameWithoutExtension(powerShellFilePath), "", functionDefinitions.Values.ToList());
        }
    }
}