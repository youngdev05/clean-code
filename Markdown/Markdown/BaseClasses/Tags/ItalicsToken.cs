using Markdown.AbstractClasses;
using Markdown.Interfaces;

namespace Markdown.Tags;

public class ItalicsToken : BaseMarkdownToken, IDoubleTag
{
    //public string Content { get; }
    //public ItalicsToken(string content)
    //{
    //    Content = content;
    //}
    public TagEnum Status { get; private set; } = TagEnum.Open;
    public override TokenNames TokenName { get; } = TokenNames.Italics;
    public ItalicsToken(TagEnum status)
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
        if (htmlResultString.Length > 0 && Status == TagEnum.Close) return ("<em>" + htmlResultString + "</em>");
        else return ("_" + htmlResultString);
    }
}