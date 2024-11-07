using Markdown.AbstractClasses;
using Markdown.Interfaces;

namespace Markdown.Tags;

public class LinkToken : BaseMarkdownToken, IDoubleTag
{
    public override TokenNames TokenName { get; } = TokenNames.LinkStart;
    public TagEnum Status { get; private set; } = TagEnum.Open;
    public LinkToken(TokenNames tokenName)
    {
        TokenName = tokenName;
    }
    public LinkToken(TagEnum status)
    {
        Status = status;
    }
    public void ChangeStatus(TagEnum status)
    {
        Status = status;
    }
    public override string ToHtml()
    {
        var htmlResultString = string.Join("", Children.Select(child => child.ToHtml()));
        if (htmlResultString.Length > 0) return ("<a href=\"" + htmlResultString + "\">" + htmlResultString + "</a>");
        else return ("<" + htmlResultString);
    }
}