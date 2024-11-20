using Markdown.Classes;

namespace Markdown;

public static class Program
{
    public static void Main(string[] args)
    {
        var md = new MarkdownRenderer();
        try
        {
            Console.WriteLine( md.RenderHtml(
                "1. Первый уровень\n    1. Второй уровень\n        1. Третий уровень\n    2. Второй элемент второго уровня"));
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}