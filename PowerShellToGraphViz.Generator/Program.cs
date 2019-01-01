using PowerShellToGraph.Parser;
using PowerShellToGraphViz.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace PowerShellToGraphVizGenerator
{
    public class Program
    {
        private const string POWER_SHELL_FILE_FILTER = "*.ps1";
        private const string GRAPH_OUTPUT_FILE_NAME = "SitecorePowerShellExtensionGraph.dot";
        private const string DOT_GRAPH_FORMAT = "digraph G {{\n\tnode[ shape = plaintext ];\n\n{0}\n{1}\n\n}}";
        public const string TEMPLATES_DIR = "Templates";
        public static readonly string basePath = AppContext.BaseDirectory;

        private static void Main(string[] args)
        {
            IDictionary<string, NodeTemplate> powerShellScriptDefinitionDict = ParsePowerShellScripts(
                powerShellScriptFileSet: GetScriptFilesFromArguments(args)
            );

            File.WriteAllText(Path.Combine(basePath, GRAPH_OUTPUT_FILE_NAME),
                ToDotString(powerShellScriptDefinitionDict));
        }

        private static ISet<string> GetScriptFilesFromArguments(string[] args)
        {
            ISet<string> powerShellScriptFileSet = new HashSet<string>();
            foreach (var arg in args)
            {
                if (Directory.Exists(arg))
                {
                    foreach (var path in Directory.GetFiles(arg, POWER_SHELL_FILE_FILTER, SearchOption.AllDirectories))
                    {
                        if (!powerShellScriptFileSet.Add(path))
                        {
                            // LOG duplicate
                        }
                    }
                }
                else if (Regex.IsMatch(arg, POWER_SHELL_FILE_FILTER))
                {
                    powerShellScriptFileSet.Add(arg);
                }
                else
                {
                    // LOG ommit file
                }
            }
            return powerShellScriptFileSet;
        }

        private static IDictionary<string, NodeTemplate> ParsePowerShellScripts(ISet<string> powerShellScriptFileSet)
        {
            IDictionary<string, NodeTemplate> powerShellScriptDefinitionDict = BuildPowerShellScriptInfo(
                powerShellScriptFileSet: powerShellScriptFileSet
            );
            return FillPowerShellDependenciesFileNames(
                powerShellScriptFileSet: powerShellScriptFileSet,
                powerShellScriptDefinitionDict: powerShellScriptDefinitionDict
            );
        }

        private static IDictionary<string, NodeTemplate> BuildPowerShellScriptInfo(ISet<string> powerShellScriptFileSet)
        {
            IDictionary<string, NodeTemplate> powerShellScriptDefinitionDict = new Dictionary<string, NodeTemplate>();
            foreach (string powerShellScriptFileName in powerShellScriptFileSet)
            {
                powerShellScriptDefinitionDict.Add(powerShellScriptFileName,
                    PowerShellParser.Parse(powerShellScriptFileName));
            }
            return powerShellScriptDefinitionDict;
        }

        private static IDictionary<string, NodeTemplate> FillPowerShellDependenciesFileNames(ISet<string> powerShellScriptFileSet,
            IDictionary<string, NodeTemplate> powerShellScriptDefinitionDict)
        {
            IDictionary<string, string> funcionFileDict = new Dictionary<string, string>();

            foreach (string powerShellScriptFilePath in powerShellScriptFileSet)
            {
                string powerShellScriptFileName = powerShellScriptDefinitionDict[powerShellScriptFilePath].FileName;
                foreach (Function functionDef in powerShellScriptDefinitionDict[powerShellScriptFilePath].Functions)
                {
                    funcionFileDict.Add(functionDef.Name, powerShellScriptFileName);
                }
            }

            foreach (string powerShellScriptFilePath in powerShellScriptFileSet)
            {
                foreach (Function functionDef in powerShellScriptDefinitionDict[powerShellScriptFilePath].Functions)
                {
                    foreach (Dependency functionDependency in functionDef.DependsOn)
                    {
                        functionDependency.FileName = funcionFileDict.ContainsKey(functionDependency.Function) 
                            ? funcionFileDict[functionDependency.Function] 
                            : null;
                    }
                }
            }
            return powerShellScriptDefinitionDict;
        }

        private static string ToDotString(IDictionary<string, NodeTemplate> powerShellScriptDefinitionDict)
        {
            StringBuilder nodeStringBuilder = new StringBuilder();
            StringBuilder edgeStringBuilder = new StringBuilder();

            foreach (var keyValuePair in powerShellScriptDefinitionDict)
            {
                NodeTemplate powerShellFileDef = keyValuePair.Value;

                nodeStringBuilder.Append(
                    new Templates.NodeTemplate(powerShellFileDef).FillTemplate()
                    + Environment.NewLine
                    + Environment.NewLine
                );

                string powerShellFileName = powerShellFileDef.FileName;
                foreach (Function functionDef in powerShellFileDef.Functions)
                {
                    string functionName = functionDef.Name;
                    foreach (Dependency functionDependency in functionDef.DependsOn)
                    {
                        edgeStringBuilder.AppendFormat(
                            "\"{0}\":\"{1}\" -> \"{2}\":\"{3}\";\n",
                            powerShellFileName,
                            functionName,
                            functionDependency.FileName,
                            functionDependency.Function
                        );
                    }
                }
            }
            return string.Format(DOT_GRAPH_FORMAT,
                nodeStringBuilder.ToString(),
                edgeStringBuilder.ToString());
        }
    }
}
