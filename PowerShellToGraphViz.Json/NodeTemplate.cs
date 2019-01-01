using System.Collections.Generic;

namespace PowerShellToGraphViz.Json
{
    public class NodeTemplate
    {

        public string FileName { get; set; }
        public IList<Function> Functions { get; set; }

        public NodeTemplate(string fileName, IList<Function> functions)
        {
            FileName = fileName;
            Functions = functions;
        }
    }
}
