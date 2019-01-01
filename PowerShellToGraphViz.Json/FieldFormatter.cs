using System.Text;

namespace PowerShellToGraphViz.Json
{
    internal static class FieldFormatter
    {
        internal static string FormatDescription(string description)
        {
            return FormatDescription(description, 40);
        }

        internal static string FormatType(string type)
        {
            while (type.StartsWith('['))
            {
                type = type.Substring(1);
            }

            while (type.EndsWith(']'))
            {
                type = type.Substring(0, type.Length - 1);
            }

            int firstGenericTypePos = type.IndexOf('[');
            if (firstGenericTypePos > 0)
            {
                type = type.Substring(0, firstGenericTypePos - 1);
            }

            int lastDotPos = type.LastIndexOf('.');
            if (lastDotPos > 0)
            {
                type = type.Substring(lastDotPos + 1);
            }

            return type;
        }

        private static string FormatDescription(string description, int maxLineLength)
        {
            StringBuilder sb = new StringBuilder();
            string[] words = description.Split(' ');
            string line = string.Empty;

            foreach (var word in words)
            {
                if ((line + word).Length > maxLineLength)
                {
                    sb.AppendLine(line);
                    line = string.Empty;
                }

                line += string.Format("{0} ", word);
            }

            if (line.Length > 0)
            {
                sb.AppendLine(line);
            }

            return sb.ToString();
        }
    }
}
