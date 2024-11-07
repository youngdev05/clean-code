namespace Markdown.AbstractClasses;
public abstract class BaseMarkdownToken
{
    public List<BaseMarkdownToken> Children { get; } = new List<BaseMarkdownToken>();
    public void AddChild(BaseMarkdownToken child)
    {
        Children.Add(child);
    }
    public abstract TokenNames TokenName { get; }
    public abstract string ToHtml();
}