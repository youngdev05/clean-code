namespace MarkDownProcessor;

public class MarkdownProcessor : IMarkdownProcessor
{
    public string Parse(string markdownText)
    {
        if (string.IsNullOrEmpty(markdownText))
            return string.Empty;

        if (markdownText.StartsWith("# "))
        {
            return $"<h1>{markdownText.Substring(2)}</h1>";
        }

        // преобразование ссылок вручную, без использования регулярных выражений
        return ParseLinks(markdownText);
    }

    private string ParseLinks(string text)
    {
        var result = new System.Text.StringBuilder();
        int i = 0;

        while (i < text.Length)
        {
            if (text[i] == '[')
            {
                int closingBracketIndex = text.IndexOf(']', i);
                if (closingBracketIndex > i + 1 && closingBracketIndex < text.Length - 1 && text[closingBracketIndex + 1] == '(')
                {
                    int closingParenthesisIndex = text.IndexOf(')', closingBracketIndex + 2);
                    if (closingParenthesisIndex > closingBracketIndex + 2)
                    {
                        // текст ссыоки
                        string linkText = text.Substring(i + 1, closingBracketIndex - i - 1);
                        string linkUrl = text.Substring(closingBracketIndex + 2, closingParenthesisIndex - closingBracketIndex - 2);
                        
                        result.Append($"<a href=\"{linkUrl}\">{linkText}</a>");
                        
                        // указатель вперед
                        i = closingParenthesisIndex + 1;
                        continue;
                    }
                }
            }

            result.Append(text[i]);
            i++;
        }

        return result.ToString();
    }

    public bool Validate(string markdownText)
    {
        if (string.IsNullOrEmpty(markdownText))
            return false;

        if (markdownText.StartsWith("# "))
            return true;

        return IsValidLink(markdownText);
    }
    private bool IsValidLink(string text)
    {
        int i = 0;

        while (i < text.Length)
        {
            if (text[i] == '[')
            {
                int closingBracketIndex = text.IndexOf(']', i);
                if (closingBracketIndex > i + 1 && closingBracketIndex < text.Length - 1 && text[closingBracketIndex + 1] == '(')
                {
                    int closingParenthesisIndex = text.IndexOf(')', closingBracketIndex + 2);
                    if (closingParenthesisIndex > closingBracketIndex + 2)
                    {
                        return true; // сслыка найдена 
                    }
                }
            }
            i++;
        }

        return false; // ссылок не найдено 
    }
}

