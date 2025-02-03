using Markdown.Classes;
using Markdown.Classes.Abstract_Classes;
using Markdown.Classes.Tags;
using Markdown.Enums;
using Xunit;

namespace Markdown.Tests
{
    public class TokenResolverTests
    {
        private readonly TokenResolver _resolver = new();

        [Fact]
        public void ResolveTokens_PairedTags_ReturnsCorrectTokens()
        {
            // Arrange
            var tokens = new Token[]
            {
                new Bold { Position = TagPosition.Start },
                new Text("жирный текст"),
                new Bold { Position = TagPosition.End }
            };

            // Act
            var resolvedTokens = _resolver.ResolveTokens(tokens);

            // Assert
            Assert.Equal(3, resolvedTokens.Length);
            Assert.IsType<Bold>(resolvedTokens[0]);
            Assert.IsType<Text>(resolvedTokens[1]);
            Assert.IsType<Bold>(resolvedTokens[2]);
        }

        [Fact]
        public void ResolveTokens_UnpairedTags_ConvertsToText()
        {
            // Arrange
            var tokens = new Token[]
            {
                new Bold { Position = TagPosition.Start },
                new Text("незакрытый тег")
            };

            // Act
            var resolvedTokens = _resolver.ResolveTokens(tokens);

            // Assert
            Assert.Equal(2, resolvedTokens.Length);
            Assert.IsType<Text>(resolvedTokens[0]);
            Assert.Equal("__", resolvedTokens[0].ToString());
            Assert.IsType<Text>(resolvedTokens[1]);
        }
    }
}