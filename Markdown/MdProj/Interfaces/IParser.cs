using Markdown.Classes.Abstract_Classes;

namespace Markdown.Interfaces;

public interface IParser
{
    public Token[] Parse(string text);
}