using System.Text;
using Markdown.Classes.Abstract_Classes;
using Markdown.Classes.Tags;
using Markdown.Enums;
using Markdown.Interfaces;

namespace Markdown.Classes;

public class TokenResolver : ITagsResolver
{
    private readonly List<TagToken> _processedTags = new();

    private void MarkAsClosed(Token[] tokens)
    {
        if (tokens.FirstOrDefault() is TagToken firstTag)
        {
            _processedTags.Add(firstTag);
            _processedTags.Add(firstTag.Pair);
        }
    }

    private Token[] ProcessOrderedLists(Token[] tokens)
    {
        if (tokens.Length == 0) return tokens;
        var tokenList = new LinkedList<Token>(tokens);
        int openLists = 0;

        for (var node = tokenList.First; node != null; node = node.Next)
        {
            if (node.Value is not ListToken currentList) continue;
            var prevList = node.Previous?.Value as ListToken;

            if (prevList is null || prevList.Offset < currentList.Offset)
            {
                tokenList.AddBefore(node, new OrderedList { Position = TagPosition.Start });
                openLists++;
            }
            else if (prevList.Offset > currentList.Offset)
            {
                tokenList.AddBefore(node, new OrderedList { Position = TagPosition.End });
                openLists--;
            }
        }

        while (openLists-- > 0)
            tokenList.AddLast(new OrderedList { Position = TagPosition.End });

        return tokenList.ToArray();
    }

    private Token[] PairTokens(Token[] tokens)
    {
        var stack = new Stack<TagToken>();

        foreach (var token in tokens.OfType<TagToken>())
        {
            if (_processedTags.Contains(token)) continue;

            if (token.Type is TagType.Paragraph or TagType.Header)
                _processedTags.Add(token);

            if (stack.TryPeek(out var prev) && prev.Type == token.Type && prev.Position == TagPosition.Start && token.Position == TagPosition.End)
            {
                _processedTags.Add(prev);
                _processedTags.Add(token);
                prev.Pair = token;
                token.Pair = prev;
                stack.Pop();
            }
            else
            {
                stack.Push(token);
            }
        }

        return tokens;
    }

    private Token[] HandleTagInteractions(Token[] tokens)
    {
        var tagList = tokens.OfType<TagToken>().ToList();

        for (int i = 0; i < tagList.Count; i++)
        {
            var currentTag = tagList[i];
            if (currentTag.Type is TagType.Header or TagType.Paragraph or TagType.ListItem or TagType.OrderedList) 
                continue;

            if (_processedTags.Contains(currentTag) && currentTag.Position == TagPosition.Start)
            {
                for (int j = i + 1; j < tagList.Count; j++)
                {
                    var nextTag = tagList[j];
                    if (currentTag.Pair == nextTag) break;
                    if (!_processedTags.Contains(nextTag)) continue;
                    if (currentTag.Type == TagType.Bold && nextTag.Type == TagType.Italic) continue;

                    _processedTags.Remove(nextTag);
                    _processedTags.Remove(nextTag.Pair!);
                }
            }
        }

        return tokens;
    }

    private Token[] ConvertUnclosedTagsToText(Token[] tokens)
    {
        return tokens.Select(t => t is TagToken tag && !_processedTags.Contains(tag) 
            ? new Text(tag.MarkdownTag) 
            : t).ToArray();
    }

    public Token[] ResolveTokens(Token[] tokens)
    {
        return ConvertUnclosedTagsToText(HandleTagInteractions(PairTokens(ProcessOrderedLists(PairTokens(tokens)))));
    }

    public Token[] ResolveTokensLines(Token[][] tokenLines)
    {
        return tokenLines.SelectMany(line =>
        {
            MarkAsClosed(line);
            return ResolveTokens(line);
        }).ToArray();
    }
}
