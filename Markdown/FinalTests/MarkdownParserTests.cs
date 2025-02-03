using Markdown.Classes;
using Markdown.Classes.Abstract_Classes;
using Markdown.Classes.Tags;
using Markdown.Enums;
using Xunit;

namespace Markdown.Tests
{
    public class MarkdownParserTests
    {
        private readonly MarkdownParser _parser = new();

        [Fact]
        public void Parse_Header_ReturnsCorrectTokens()
        {
            // Arrange
            var input = "# Заголовок";

            // Act
            var tokens = _parser.Parse(input);

            // Assert
            Assert.Equal(3, tokens.Length);
            Assert.IsType<Header>(tokens[0]);
            Assert.Equal(TagPosition.Start, ((TagToken)tokens[0]).Position);
            Assert.IsType<Text>(tokens[1]);
            Assert.Equal("Заголовок", tokens[1].ToString());
            Assert.IsType<Header>(tokens[2]);
            Assert.Equal(TagPosition.End, ((TagToken)tokens[2]).Position);
        }

        [Fact]
        public void Parse_BoldText_ReturnsCorrectTokens()
        {
            // Arrange
            var input = "__жирный текст__";

            // Act
            var tokens = _parser.Parse(input);

            // Assert
            Assert.Equal(3, tokens.Length);
            Assert.IsType<Bold>(tokens[0]);
            Assert.Equal(TagPosition.Start, ((TagToken)tokens[0]).Position);
            Assert.IsType<Text>(tokens[1]);
            Assert.Equal("жирный текст", tokens[1].ToString());
            Assert.IsType<Bold>(tokens[2]);
            Assert.Equal(TagPosition.End, ((TagToken)tokens[2]).Position);
        }

        [Fact]
        public void Parse_ListItem_ReturnsCorrectTokens()
        {
            // Arrange
            var input = "1. Элемент списка";

            // Act
            var tokens = _parser.Parse(input);

            // Assert
            Assert.Equal(4, tokens.Length);
            Assert.IsType<ListItem>(tokens[0]);
            Assert.Equal(TagPosition.Start, ((TagToken)tokens[0]).Position);
            Assert.IsType<Text>(tokens[1]);
            Assert.Equal("Элемент", tokens[1].ToString());
            Assert.IsType<Text>(tokens[2]);
            Assert.Equal("списка", tokens[2].ToString());
            Assert.IsType<ListItem>(tokens[3]);
            Assert.Equal(TagPosition.End, ((TagToken)tokens[3]).Position);
        }
    }
}