using System;

namespace PowerShellToGraphViz.Json
{
    public class Parameter
    {
        private string type;

        public bool Reqiuired { get; set; }
        public string Type {
            get => FieldFormatter.FormatType(type);
            set => type = value;
        }
        public string Name { get; set; }

        public Parameter(Type type, string name)
        {
            Reqiuired = false;
            Type = type.Name;
            Name = name;
        }

        public Parameter(bool reqiuired, string type, string name)
        {
            Reqiuired = reqiuired;
            Type = type;
            Name = name;
        }
    }
}