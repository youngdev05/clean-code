using Markdown.Classes;
using Markdown.Classes.Abstract_Classes;
using Markdown.Classes.Tags;
using Markdown.Enums;
using Xunit;

namespace Markdown.Tests
{
    public class TagLibraryTests
    {
        [Fact]
        public void GetTagToken_Header_ReturnsHeaderToken()
        {
            // Arrange
            var tag = "#";

            // Act
            var token = TagLibrary.GetTagToken(tag);

            // Assert
            Assert.IsType<Header>(token);
            Assert.Equal((TagPosition)1, ((Header)token).Position);
        }

        [Fact]
        public void GetTagToken_Bold_ReturnsBoldToken()
        {
            // Arrange
            var tag = "__";

            // Act
            var token = TagLibrary.GetTagToken(tag);

            // Assert
            Assert.IsType<Bold>(token);
        }
    }
}