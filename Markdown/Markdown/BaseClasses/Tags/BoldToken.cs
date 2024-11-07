using Markdown.AbstractClasses;
using Markdown.Interfaces;

namespace Markdown.Tags;

public class BoldToken : BaseMarkdownToken, IDoubleTag
{
    //public string Content { get; }
    //public BoldToken(string content)
    //{
    //    Content = content;
    //}
    public override TokenNames TokenName { get; } = TokenNames.Bold;
    public TagEnum Status { get; private set; } = TagEnum.Open;
    public BoldToken(TagEnum status) 
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
        if (htmlResultString.Length > 0 && Status == TagEnum.Close) return ("<strong>" + htmlResultString + "</strong>");
        else return ("__" + htmlResultString);
    }
}