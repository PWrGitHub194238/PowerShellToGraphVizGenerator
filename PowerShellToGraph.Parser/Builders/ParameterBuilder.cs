using PowerShellToGraphViz.Json;
using System.Linq;
using System.Management.Automation.Language;

namespace PowerShellToGraph.Parser.Builders
{
    public class ParameterBuilder
    {
        private bool required;
        private readonly string name;
        private readonly string type;

        public ParameterBuilder(ParameterAst parameter)
        {
            name = parameter.Name.Extent.Text;
            type = IsSwitch(parameter)
                ? PsTokens.PARAMETER_ATTR_SWITCH_LOWER_TOKEN
                : parameter.StaticType.Name;
        }

        private bool IsSwitch(ParameterAst parameter)
        {
            TypeConstraintAst switchAttr = (TypeConstraintAst)parameter.Attributes
                .Where(a => PsTokens.IsEqual(a.TypeName.Name, PsTokens.PARAMETER_ATTR_SWITCH_LOWER_TOKEN))
                .FirstOrDefault();
            return switchAttr != null;
        }

        public ParameterBuilder IsRequired(NamedAttributeArgumentAst parameterArgument)
        {
            required = PsTokens.IsEqual(parameterArgument.Argument, PsTokens.TRUE_LOWER_TOKEN);
            return this;
        }

        public Parameter Build()
        {
            return new Parameter(
                reqiuired: required,
                type: type,
                name: name);
        }
    }
}
