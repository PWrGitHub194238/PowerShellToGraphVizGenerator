namespace PowerShellToGraphViz.Json
{
    public class Dependency
    {
        public string FileName { get; set; }
        public string Function { get; set; }

        public Dependency(string function)
        {
            Function = function;
        }
    }
}