using System.Text;
using Markdown.Classes;
using MdProcessor.Abstract_Classes;
using MdProcessor.Enums;
using MdProcessor.Interfaces;

namespace MdProcessor.Classes;

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