using Markdown.Enums;

namespace Markdown.Classes.Abstract_Classes;

public abstract class TagToken : Token, ICloneable
{
    public abstract string HtmlTag { get; }
    public abstract string MarkdownTag { get; }
    public abstract TagType Type { get; }
    public abstract TagPosition Position { get; set; }
    public abstract TagToken? Pair {  get; set; }
        
        
    public abstract object Clone();
}