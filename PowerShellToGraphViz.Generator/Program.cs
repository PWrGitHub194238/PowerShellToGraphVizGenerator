using GraphVizWrapper;
using GraphVizWrapper.Commands;
using GraphVizWrapper.Queries;
using PowerShellToGraph.Parser;
using PowerShellToGraphViz.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PowerShellToGraphVizGenerator
{
    public class Program
    {
        public const string TEMPLATES_DIR = "Templates";
        public static readonly string basePath = AppContext.BaseDirectory;

        private static void Main(string[] args)
        {
            string libPath = args[0];
            string[] allfiles = Directory.GetFiles(libPath, "*.ps1", SearchOption.AllDirectories);

            IDictionary<string, NodeTemplate> psScriptFiles = new Dictionary<string, NodeTemplate>();

            foreach(var file in allfiles)
            {
                NodeTemplate t = PowerShellParser.Parse(file);
                psScriptFiles.Add(file, t);
            }

            IDictionary<string, string> functionsInFile = new Dictionary<string, string>();

            foreach (var file in allfiles)
            {
                foreach (var function in psScriptFiles[file].Functions)
                {
                    functionsInFile.Add(function.Name, psScriptFiles[file].FileName);
                }
            }


            foreach (var file in allfiles)
            {
                foreach (var function in psScriptFiles[file].Functions)
                {
                    foreach (var dependency in function.DependsOn)
                    {
                        dependency.FileName = functionsInFile[dependency.Function];
                    }
                }
            }

            //Templates.NodeTemplate tt = new Templates.NodeTemplate(t);

            //string o = tt.FillTemplate();

            StringBuilder sb = new StringBuilder();

            foreach (var script in psScriptFiles.Values)
            {
                sb.Append(new Templates.NodeTemplate(script).FillTemplate() + "\n\n");
            }
            string o = sb.ToString();

            sb = new StringBuilder();
            foreach (var script in psScriptFiles.Values)
            {
                foreach (var functions in script.Functions)
                {
                    foreach (var dependency in functions.DependsOn)
                    {
                        sb.AppendFormat("\"{0}\":\"{1}\" -> \"{2}\":\"{3}\";\n", script.FileName, functions.Name, dependency.FileName, dependency.Function);
                    }
                }
            }
            string links = sb.ToString();

            // These three instances can be injected via the IGetStartProcessQuery, 
            //                                               IGetProcessStartInfoQuery and 
            //                                               IRegisterLayoutPluginCommand interfaces

            var getStartProcessQuery = new GetStartProcessQuery();
            var getProcessStartInfoQuery = new GetProcessStartInfoQuery();
            var registerLayoutPluginCommand = new RegisterLayoutPluginCommand(getProcessStartInfoQuery, getStartProcessQuery);

            // GraphGeneration can be injected via the IGraphGeneration interface

            var wrapper = new GraphGeneration(getStartProcessQuery,
                                              getProcessStartInfoQuery,
                                              registerLayoutPluginCommand);
            string s = "digraph G { node[ shape = plaintext ]; " + o + "\n\n" + links + "  }";
            File.WriteAllText(Path.Combine(basePath, "output.dot"), s);
            wrapper.RenderingEngine = Enums.RenderingEngine.Dot;
            byte[] output = wrapper.GenerateGraph(Path.Combine(basePath, "output.dot"), Enums.GraphReturnType.Svg);
            File.WriteAllBytes(Path.Combine(basePath, "output.svg"), output);
        }
    }
}
