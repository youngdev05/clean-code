using Markdown.AbstractClasses;

namespace Markdown.Tags;

public class MainToken : BaseMarkdownToken
{
    public override TokenNames TokenName { get; } = TokenNames.Main;
    public override string ToHtml()
    {
        var htmlResultString = string.Join("\n", Children.Select(child => child.ToHtml()));
        return (htmlResultString);
    }
}