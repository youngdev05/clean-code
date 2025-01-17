using MdProcessor.Interfaces;
using MdProcessor.Abstract_Classes;
using MdProcessor.Enums;
using MdProcessor.Tags;

namespace MdProcessor.Classes;

public class MarkdownParser : IParser
{
    private int CountLeadingSpaces(string line)
    {
        int count = 0;
        while (count < line.Length && line[count] == ' ')
            count++;
        
        return count / 4;
    }

    private (string[] Words, TagToken InitialTag) IdentifyOpeningTag(string line)
    {
        var indentation = CountLeadingSpaces(line);
        var words = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
        if (words.Length == 0)
            return (new[] { " " }, new Paragraph());

        return words.First() switch
        {
            "1." or "2." or "3." or "4." or "5." or "6." or "7." or "8." or "9." 
                => (words.Skip(1).ToArray(), new ListItem(indentation)),
            "#" or "##" or "###" or "####" or "#####" or "######" 
                => (words.Skip(1).ToArray(), TagLibrary.GetTagToken(words.First())),
            _ 
                => (words, new Paragraph())
        };
    }

    private Token[] ProcessWord(string word)
    {
        var tokens = new List<Token>();

        if (word.All(c => c == '_'))
        {
            tokens.Add(new Text(word));
            return tokens.ToArray();
        }
        
        string currentWord = word;
        foreach (var tag in TagLibrary.Tags)
        {
            if (currentWord.StartsWith(tag))
            {
                tokens.Add(TagLibrary.GetTagToken(tag));
                currentWord = currentWord[tag.Length..];
            }
        }

        var closingTags = new Stack<Token>();
        foreach (var tag in TagLibrary.Tags)
        {
            if (currentWord.EndsWith(tag))
            {
                var endTag = TagLibrary.GetTagToken(tag);
                endTag.Position = TagPosition.End;
                currentWord = currentWord[..^tag.Length];
                closingTags.Push(endTag);
            }
        }

        tokens.Add(new Text(currentWord));
        while (closingTags.Count > 0)
        {
            tokens.Add(closingTags.Pop());
        }
        
        return tokens.ToArray();
    }

    public Token[] Parse(string input)
    {
        var tokens = new List<Token>();
        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            var parsedTokens = new List<Token>();
            var (remainingWords, startTag) = IdentifyOpeningTag(line);
            parsedTokens.Add(startTag);

            foreach (var word in remainingWords)
            {
                if (word != remainingWords.First())
                    parsedTokens.Add(new Text(" "));
                
                parsedTokens.AddRange(ProcessWord(word));
            }

            var endTag = (TagToken)startTag.Clone();
            endTag.Position = TagPosition.End;
            endTag.Pair = startTag;
            startTag.Pair = endTag;
            parsedTokens.Add(endTag);

            tokens.AddRange(parsedTokens);
        }

        return tokens.ToArray();
    }
}
