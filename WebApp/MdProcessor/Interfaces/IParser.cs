using MdProcessor.Abstract_Classes;

namespace MdProcessor.Interfaces;

public interface IParser
{
    public Token[] Parse(string text);
}