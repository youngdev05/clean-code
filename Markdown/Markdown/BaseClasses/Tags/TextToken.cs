using Markdown.AbstractClasses;

namespace Markdown.Tags;

public class TextToken : BaseMarkdownToken
{
    public override TokenNames TokenName { get; } = TokenNames.Text;
    public string Content { get; }
    public TextToken(string content)
    {
        Content = content;
    }
    public override string ToHtml()
    {
        return Content;
    }
}