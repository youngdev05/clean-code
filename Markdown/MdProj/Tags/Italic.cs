using Markdown.Classes.Abstract_Classes;
using Markdown.Enums;

namespace Markdown.Classes.Tags;

public class Italic : TagToken
{
    public override string HtmlTag => "em";
    public override string MarkdownTag => "_";
    public override TagType Type => TagType.Italic;
    public override TagPosition Position { get; set; }
    public override TagToken? Pair { get; set; }

    public override object Clone() => new Italic()
    {
        Pair = (TagToken?)Pair?.Clone()
    };
}