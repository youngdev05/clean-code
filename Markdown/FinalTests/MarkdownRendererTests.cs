using Markdown.Classes;
using Markdown.Classes.Abstract_Classes;
using Xunit;

namespace Markdown.Tests
{
    public class MarkdownRendererTests
    {
        private readonly MarkdownRenderer _renderer = new();

        [Fact]
        public void RenderHtml_Header_ReturnsCorrectHtml()
        {
            // Arrange
            var input = "# Заголовок";

            // Act
            var result = _renderer.RenderHtml(input);

            // Assert
            Assert.Equal("<h1>Заголовок</h1>", result);
        }

        [Fact]
        public void RenderHtml_BoldText_ReturnsCorrectHtml()
        {
            // Arrange
            var input = "__жирный текст__";

            // Act
            var result = _renderer.RenderHtml(input);

            // Assert
            Assert.Equal("<strong>жирный текст</strong>", result);
        }

        [Fact]
        public void RenderHtml_ListItem_ReturnsCorrectHtml()
        {
            // Arrange
            var input = "1. Элемент списка";

            // Act
            var result = _renderer.RenderHtml(input);

            // Assert
            Assert.Equal("<ol><li>Элемент списка</li></ol>", result);
        }
    }
}