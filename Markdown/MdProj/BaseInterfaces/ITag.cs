namespace Markdown.BaseInterfaces;

public interface ITag: IResetTag
{
    string Symbol { get; }
    bool IsPaired { get; }
    
}