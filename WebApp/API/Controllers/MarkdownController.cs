using Microsoft.AspNetCore.Mvc;
using MdProcessor.Classes; // Подключаем твой процессор

[ApiController]
[Route("api/markdown")]
public class MarkdownController : ControllerBase
{
    private readonly MarkdownParser _parser = new(); // Создаём экземпляр процессора

    [HttpPost("parse")]
    public IActionResult ParseMarkdown([FromBody] MarkdownRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
            return BadRequest("Пустой Markdown");

        var tokens = _parser.Parse(request.Content);
        var html = string.Join("", tokens.Select(t => t.ToString())); // Метод ToHtml() нужно реализовать в Token

        return Ok(new { html });
    }
}

public class MarkdownRequest
{
    public string Content { get; set; } = string.Empty;
}