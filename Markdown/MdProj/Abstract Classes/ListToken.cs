namespace Markdown.Classes.Abstract_Classes;

public abstract class ListToken(int offset) : TagToken
{
    public abstract int Offset { get; }
}