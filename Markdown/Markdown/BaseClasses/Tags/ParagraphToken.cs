using Markdown.AbstractClasses;

namespace Markdown.Tags;

public class ParagraphToken : BaseMarkdownToken
{
    public override TokenNames TokenName { get; } = TokenNames.Paragraph;
    public override string ToHtml()
    {
        // Дополнительно разделяем пробелами "слова"
        var htmlResultString = string.Join("", Children.Select((child, i) => i != 0 ? " " + child.ToHtml() : child.ToHtml()));
        return "<p>" + htmlResultString + "</p>";
    }
}