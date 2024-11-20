using Markdown.Classes.Abstract_Classes;
using Markdown.Enums;

namespace Markdown.Classes.Tags;

public class OrderedList : TagToken
{
    public override string HtmlTag => "ol";
    public override string MarkdownTag => "";
    public override TagType Type => TagType.OrderedList;
    public override TagPosition Position { get; set; }
    public override TagToken? Pair { get; set; }
    public override object Clone() => new OrderedList()
    {
        Pair = (TagToken?)Pair?.Clone()
    };
}