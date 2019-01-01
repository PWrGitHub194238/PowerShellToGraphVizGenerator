using System.Collections.Generic;

namespace PowerShellToGraphViz.Json
{
    public class Function
    {
        private string description;

        public string Scope { get; set; }
        public string Name { get; set; }
        public string Description {
            get => FieldFormatter.FormatDescription(description);
            set => description = value;
        }
        public IList<Parameter> Parameters { get; set; }
        public IList<Dependency> DependsOn { get; set; }

        public Function(string scope, string name, string description)
        {
            Scope = scope;
            Name = name;
            Description = description;
        }
    }
}