using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using PowerShellToGraphVizGenerator.Templates;

namespace PowerShellToGraphVizGenerator
{
    public class FillTemplateUtils
    {
        internal static string FillByList(string inputString, IEnumerable<DotTemplateItem> templateItemList,
            string templatePlaceholderName)
        {
            StringBuilder sb = new StringBuilder();

            foreach (DotTemplateItem templateItem in templateItemList)
            {
                sb.AppendLine(templateItem.FillTemplate());
            }
            return inputString.Replace(oldValue: templatePlaceholderName, newValue: sb.ToString().TrimEnd());
        }
    }
}
