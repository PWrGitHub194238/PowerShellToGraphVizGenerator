using PowerShellToGraph.Parser.Builders;
using PowerShellToGraphViz.Extensions.Management.Automation.Language;
using PowerShellToGraphViz.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation.Language;
using System.Text.RegularExpressions;

namespace PowerShellToGraph.Parser
{
    public static class PowerShellParser
    {
        private const string FUNCTION_WITHOUT_DOC_FORMAT = @"Function {0} has no documentation.";

        private static readonly Regex _synopsisRegexp = new Regex(
            @"\.SYNOPSIS\s(.*?)(\.DESCRIPTION|\.PARAMETER|\.EXAMPLE|\.INPUTS|\.OUTPUTS|\.NOTES|\.LINK|#>)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static NodeTemplate Parse(string powerShellFilePath)
        {
            NodeTemplate powerShellScriptDef = null;
            if (TryParse(powerShellFilePath, out IReadOnlyCollection<StatementAst> statements, out Token[] tokens))
            {


                IList<FunctionDefinitionAst> functionDefList = GetFunctions(
                    statements: statements);

                IList<string> importList = GetImports(statements: statements);

                IDictionary<string, string> functionCommentHelpDict = GetSynopsis(
                    functionDefList: functionDefList, tokens: tokens);

                IDictionary<string, Function> functionDefinitions = GetFunctionDefinitions(
                    functionDefList: functionDefList, functionCommentHelpDict: functionCommentHelpDict);

                AddImportsWithinFunctions(GetImports: importList, functionDefinitions: functionDefinitions);

                foreach (FunctionDefinitionAst functionDef in functionDefList)
                {
                    IList<Parameter> functionParamList = new List<Parameter>();

                    if (FunctionHasParameters(functionDef: functionDef))
                    {
                        IReadOnlyCollection<ParameterAst> parameters = functionDef.Body.ParamBlock.Parameters;

                        foreach (ParameterAst parameter in parameters)
                        {
                            ParameterBuilder parameterBuilder = new ParameterBuilder(parameter);
                            AttributeAst parameterAttributes = GetAttributes(parameter);

                            foreach (var parameterArgument in parameterAttributes.NamedArguments)
                            {
                                string argumentName = PsTokens.GetTokenFormat(parameterArgument.ArgumentName);
                                switch (argumentName)
                                {
                                    case PsTokens.PARAMETER_ATTR_MANDATORY_TOKEN:
                                        parameterBuilder = parameterBuilder.IsRequired(parameterArgument);
                                        break;
                                }
                            }
                            functionParamList.Add(parameterBuilder.Build());
                        }
                    }
                }
                //foreach (FunctionDefinitionAst fun in functions)
                //{

                //    IList<Parameter> parameterList = new List<Parameter>();
                //    var name = fun.Name;

                //    if (name.Contains(':'))
                //    {
                //        name = name.Substring(name.IndexOf(':') + 1);
                //    }
                //    if (fun.Body.ParamBlock != null)
                //    {
                //        var parameters = fun.Body.ParamBlock.Parameters;

                //        foreach (ParameterAst parameter in parameters)
                //        {
                //            var paramIsReq = false;

                //            AttributeAst parameterAttr = (AttributeAst)parameter.Attributes.Where(a => a.TypeName.Name.ToLower().Equals("parameter")).FirstOrDefault();
                //            if (parameterAttr != null)
                //            {
                //                var val = parameterAttr.NamedArguments.Where(a => a.ArgumentName.ToLower().Equals("mandatory")).FirstOrDefault();
                //                var str = val.Argument.ToString();
                //                paramIsReq = val.Argument.ToString().Equals("$true");
                //                var paramName = parameter.Name.Extent.Text;
                //                var paramType = parameter.StaticType.Name;
                //                parameterList.Add(new Parameter(paramIsReq, paramType, paramName));
                //            }
                //            TypeConstraintAst switchAttr = (TypeConstraintAst)parameter.Attributes.Where(a => a.TypeName.Name.ToLower().Equals("switch")).FirstOrDefault();
                //            if (switchAttr != null)
                //            {
                //                parameterList.Add(new Parameter(false, "switch", parameter.Name.Extent.Text));
                //            }
                //        }
                //    }
                //    IList<Dependency> dependsOnList = new List<Dependency>();

                //    foreach (string import in imports)
                //    {
                //        if (fun.Body.Extent.Text.Contains(import))
                //        {
                //            dependsOnList.Add(new Dependency(import));
                //        }
                //    }

                //    functionDefinitions[name].Parameters = parameterList;
                //    functionDefinitions[name].DependsOn = dependsOnList;
                //    }
                //    powerShellScriptDef = new NodeTemplate(Path.GetFileNameWithoutExtension(powerShellFilePath), functionDefinitions.Values.ToList()); ;
            }

            return powerShellScriptDef;
        }

        private static AttributeAst GetAttributes(ParameterAst parameter)
        {
            return (AttributeAst)parameter.Attributes
                .Where(a => PsTokens.IsEqual(a.TypeName.Name, PsTokens.PARAMETER_TOKEN))
                .FirstOrDefault();

        }

        private static bool FunctionHasParameters(FunctionDefinitionAst functionDef)
        {
            return functionDef.Body?.ParamBlock?.Parameters != null;
        }

        private static void AddImportsWithinFunctions(IList<string> GetImports,
            IDictionary<string, Function> functionDefinitions)
        {
            foreach (string functionName in functionDefinitions.Keys)
            {
                GetImports.Add(functionName);
            }
        }

        private static IDictionary<string, Function> GetFunctionDefinitions(IList<FunctionDefinitionAst> functionDefList, IDictionary<string, string> functionCommentHelpDict)
        {
            IDictionary<string, Function> functionDefinitions = new Dictionary<string, Function>();
            foreach (FunctionDefinitionAst functionDef in functionDefList)
            {
                string functionName = functionDef.GetFunctionLocalName();
                string functionSynopsis = GetFunctionSynopsisFromDict(
                    functionSynopsisDict: functionCommentHelpDict,
                    functionName: functionName);

                functionDefinitions.Add(functionName,
                    new Function(functionDef.GetFunctionScopeName(),
                        functionName, functionSynopsis));
            }
            return functionDefinitions;
        }

        private static string GetFunctionSynopsisFromDict(IDictionary<string, string> functionSynopsisDict,
            string functionName)
        {
            return functionSynopsisDict.ContainsKey(functionName)
                ? functionSynopsisDict[functionName]
                : string.Format(FUNCTION_WITHOUT_DOC_FORMAT, functionName);
        }

        private static IList<string> GetImports(IReadOnlyCollection<StatementAst> statements)
        {
            return GetDefinitions<PipelineAst, string>(statements,
                s => PsTokens.StartsWith(s, PsTokens.IMPORT_FUNCTION_TOKEN),
                s =>
                {
                    string import = s.Extent.Text.TrimEnd();
                    return import.Substring(import.LastIndexOf(' ') + 1);
                });
        }

        private static IList<T> GetDefinitions<T>(IReadOnlyCollection<StatementAst> statements)
        where T : StatementAst
        {
            return GetDefinitions<T>(statements: statements, wherePpredicate: s => true);
        }

        private static IList<T> GetDefinitions<T>(IReadOnlyCollection<StatementAst> statements,
            Func<T, bool> wherePpredicate)
        where T : StatementAst
        {
            return GetDefinitions<T, T>(statements: statements,
                wherePpredicate: s => true, selectPredicate: s => s);
        }

        private static IList<U> GetDefinitions<T, U>(IReadOnlyCollection<StatementAst> statements,
            Func<T, bool> wherePpredicate, Func<T, U> selectPredicate)
        where T : StatementAst
        {
            return statements
                .Where(s =>
                {
                    return s.GetType().Equals(typeof(T))
                        && wherePpredicate((T)s);
                })
                .Select(s =>
                    selectPredicate((T)s))
                .ToList();
        }

        private static IList<FunctionDefinitionAst> GetFunctions(IReadOnlyCollection<StatementAst> statements)
        {
            return GetDefinitions<FunctionDefinitionAst>(statements: statements);
        }

        private static IDictionary<string, string> GetSynopsis(IList<FunctionDefinitionAst> functionDefList,
            Token[] tokens)
        {
            IDictionary<string, string> functionCommentHelpDict = new Dictionary<string, string>();

            foreach (FunctionDefinitionAst functionDef in functionDefList)
            {
                Ast parent = functionDef.Parent;
                while (parent != null && !ContainsSynopsisBlock(text: parent.Extent.Text))
                {
                    parent = parent.Parent;
                }
                functionCommentHelpDict.Add(
                    functionDef.GetFunctionLocalName(),
                    parent != null
                        ? GetSynopsisFromString(functionDocComment: parent.Extent.Text)
                        : string.Empty);
            }
            return functionCommentHelpDict;
        }

        private static bool ContainsSynopsisBlock(string text)
        {
            return PsTokens.Contains(text, PsTokens.SYNOPSIS_TOKEN);
        }

        private static string GetSynopsisFromString(string functionDocComment)
        {
            Match match = _synopsisRegexp.Match(functionDocComment);

            return match.Success ? match.Groups[1].Captures[0].Value.Trim() : string.Empty;
        }

        private static bool TryParse(string powerShellFilePath,
            out IReadOnlyCollection<StatementAst> statements,
            out Token[] tokens)
        {
            ScriptBlockAst tokenTable = System.Management.Automation.Language.Parser.ParseFile(
                fileName: powerShellFilePath,
                tokens: out tokens,
                errors: out ParseError[] errors);

            statements = tokenTable?.EndBlock?.Statements;
            return errors.Length == 0 && statements != null;
        }
    }
}