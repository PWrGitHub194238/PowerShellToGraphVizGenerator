using System.Management.Automation.Language;

namespace PowerShellToGraphViz.Extensions.Management.Automation.Language
{
    public static class TokenExt
    {
        public static bool IsTokenOfKind(this Token token, TokenKind comment)
        {
            return token.Kind == comment;
        }

        public static bool IsTokenComment(this Token token)
        {
            return IsTokenOfKind(token, TokenKind.Comment);
        }
    }
}
