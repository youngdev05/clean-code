using Xunit;

public class MarkdownProcessorTests
{
    [Fact]
    public void Parse_ShouldConvertMarkdownHeadingToHtml()
    {
        // Arrange
        IMarkdownProcessor processor = new MarkdownProcessor();
        string markdown = "# Heading";

        // Act
        string result = processor.Parse(markdown);

        // Assert
        Assert.Equal("<h1>Heading</h1>", result);
    }

    [Fact]
    public void Validate_ShouldReturnTrueForValidMarkdown()
    {
        // Arrange
        IMarkdownProcessor processor = new MarkdownProcessor();
        string markdown = "# Valid Heading";

        // Act
        bool isValid = processor.Validate(markdown);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void Validate_ShouldReturnFalseForInvalidMarkdown()
    {
        // Arrange
        IMarkdownProcessor processor = new MarkdownProcessor();
        string markdown = "Invalid Heading";

        // Act
        bool isValid = processor.Validate(markdown);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void Parse_ShouldReturnOriginalText_WhenMarkdownIsInvalid()
    {
        // Arrange
        IMarkdownProcessor processor = new MarkdownProcessor();
        string markdown = "Invalid Heading";

        // Act
        string result = processor.Parse(markdown);

        // Assert
        Assert.Equal(markdown, result);
    }
}
