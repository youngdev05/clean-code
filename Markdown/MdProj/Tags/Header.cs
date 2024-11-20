using Markdown.Classes.Abstract_Classes;
using Markdown.Enums;

namespace Markdown.Classes.Tags;

public class Header : TagToken
{
    public override string HtmlTag { get; }
    public override string MarkdownTag { get; }
    public override TagType Type => TagType.Header;
    public override TagPosition Position { get; set; }
    public override TagToken? Pair { get; set; }
    private int HeaderLevel { get; init; }

    public Header(int headerLevel)
    {
        if (headerLevel is < 1 or > 6)
            throw new ArgumentException("Header level must be between 1 and 6", nameof(headerLevel));
        
        MarkdownTag = new string('#', headerLevel);
        HtmlTag = $"h{headerLevel}";
        HeaderLevel = headerLevel;
    }
    
    public override object Clone() => new Header(HeaderLevel)
    {
        Pair = (TagToken?)Pair?.Clone(),
    };
}