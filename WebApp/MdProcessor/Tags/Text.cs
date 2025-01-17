using MdProcessor.Abstract_Classes;

namespace MdProcessor.Tags;

public class Text(string content) : Token
{
    public override string ToString() => content;
}