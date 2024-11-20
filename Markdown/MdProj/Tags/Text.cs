using Markdown.Classes.Abstract_Classes;

namespace Markdown.Classes.Tags;

public class Text(string content) : Token
{
    public override string ToString() => content;
}