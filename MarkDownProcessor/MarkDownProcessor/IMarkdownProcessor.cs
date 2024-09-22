namespace MarkDownProcessor;

public interface IMarkdownProcessor
{
    string Parse(string markdownText);
    bool Validate(string markdownText);
}