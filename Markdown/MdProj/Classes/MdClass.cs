using System.Text;
using Markdown.Classes.Abstract_Classes;
using Markdown.Enums;
using Markdown.Interfaces;

namespace Markdown.Classes;

public class MarkdownRenderer : IRender
{
    private readonly MarkdownParser _parser = new();
    private readonly TokenResolver _resolver = new();
    
    public string RenderHtml(string input)
    {
        var output = new StringBuilder();
        var tokens = _resolver.ResolveTokens(_parser.Parse(input));
        
        foreach (var token in tokens)
        {
            if (token is not TagToken tag)
            {
                output.Append(token);
                continue;
            }
            
            output.Append(tag.Position == TagPosition.Start 
                ? $"<{tag.HtmlTag}>" 
                : $"</{tag.HtmlTag}>"
            );
        }
        
        return output.ToString();
    }
}