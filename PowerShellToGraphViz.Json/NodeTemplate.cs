using System.Collections.Generic;

namespace PowerShellToGraphViz.Json
{
    public class NodeTemplate
    {
        private string fileDescription;

        public string FileName { get; set; }
        public string FileDescription {
            get => FieldFormatter.FormatDescription(fileDescription);
            set => fileDescription = value;
        }
        public IList<Function> Functions { get; set; }

        public NodeTemplate(string fileName, string fileDescription, IList<Function> functions)
        {
            FileName = fileName;
            FileDescription = fileDescription;
            Functions = functions;
        }
    }
}
