namespace Markdown.BaseClasses;

public class MarkdownRenderer : IMarkdownProcessor
{
    public string Render(string markdownText)
    {
        Token token = new Token();
        string htmlString = token.Tokenize(markdownText).ToHtml();
        return htmlString;
    }
}