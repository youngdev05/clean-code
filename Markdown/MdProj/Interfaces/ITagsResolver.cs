using Markdown.Classes.Abstract_Classes;

namespace Markdown.Interfaces;

public interface ITagsResolver
{
    public Token[] ResolveTokens(Token[] tokens);
    public Token[] ResolveTokensLines(Token[][] tokensLines);
}