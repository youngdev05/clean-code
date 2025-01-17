using MdProcessor.Abstract_Classes;
using MdProcessor.Enums;

namespace MdProcessor.Tags;

public class Bold : TagToken
{
    public override string HtmlTag => "strong";
    public override string MarkdownTag => "__";
    public override TagType Type => TagType.Bold;
    public override TagPosition Position { get; set; }
    public override TagToken? Pair { get; set; }

    public override object Clone() => new Bold()
    {
        Pair = (TagToken?)Pair?.Clone(),
    };
}