using MdProcessor.Abstract_Classes;

namespace MdProcessor.Interfaces;

public interface ITagsResolver
{
    public Token[] ResolveTokens(Token[] tokens);
    public Token[] ResolveTokensLines(Token[][] tokensLines);
}