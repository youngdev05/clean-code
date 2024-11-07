using Markdown.AbstractClasses;
using Markdown.Tags;

namespace Markdown.BaseClasses;
public class Token
{
    bool isEscapingSupported;
    HashSet<string> allowedTags;
    public MainToken Tokenize(string markdownText)
    {
        if (markdownText.Length == 0)
        {
            return new MainToken();
        }
        MainToken mainToken = new MainToken();
        BaseMarkdownToken rootToken = new ParagraphToken();

        isEscapingSupported = true;
        allowedTags = isEscapingSupported ? new HashSet<string> { "_", "__", "#", "<", ">", "\\" } : new HashSet<string> { "_", "__", "#", "<", ">" };

        string[] markdownTextParagraphs = markdownText.Split("\n");
        foreach (var markdownTextParagraph in markdownTextParagraphs)
        {
            var pointerToCurrentTokenStack = new Stack<BaseMarkdownToken>();
            string[] wordsMarkdownTextParagraph = markdownTextParagraph.Split(" ");

            if (markdownTextParagraph[0] == '#')
            {
                rootToken = new HeaderToken();
                pointerToCurrentTokenStack.Push(rootToken);
                wordsMarkdownTextParagraph[0] = wordsMarkdownTextParagraph[0].Substring(1);
            }
            else
            {
                rootToken = new ParagraphToken();
                pointerToCurrentTokenStack.Push(rootToken);
            }

            mainToken.Children.Add(rootToken);

            
            foreach (var word in wordsMarkdownTextParagraph)
            {
                pointerToCurrentTokenStack = new Stack<BaseMarkdownToken>();
                WordToken wordToken = new WordToken();
                rootToken.Children.Add(wordToken);
                pointerToCurrentTokenStack.Push(wordToken);


                Dictionary<TokenNames, Queue<int>> characterProcessingSequence = GetCharacterProcessingSequence(word);
                foreach (var tag in characterProcessingSequence)
                {
                    Console.Write(tag.Key + ":");
                    foreach (var token in tag.Value)
                    {
                        Console.Write(token);
                    }
                    Console.WriteLine();
                }
                StringBuffer readParagraphBuffer = new StringBuffer();

                for (int i = 0; i < word.Length; i++)
                {
                    if (i + 1 < word.Length && word[i] == '_' && word[i + 1] == '_' && !(word.ToString().Any(char.IsDigit) && i != 0 && i != word.Length - 1) && characterProcessingSequence[TokenNames.Bold].Count > 0 && characterProcessingSequence[TokenNames.Bold].Dequeue() == 1)
                    {
                        ManagePointerStack(word, pointerToCurrentTokenStack, TokenNames.Bold, readParagraphBuffer);
                        i += 1;
                    }
                    else if (word[i] == '_' && !(word.ToString().Any(char.IsDigit) && i!=0 && i!=word.Length-1) && characterProcessingSequence[TokenNames.Italics].Count > 0 && characterProcessingSequence[TokenNames.Italics].Dequeue() == 1)
                    {
                        ManagePointerStack(word, pointerToCurrentTokenStack, TokenNames.Italics, readParagraphBuffer);
                    }
                    else if (characterProcessingSequence[TokenNames.LinkStart].Count > 0 && characterProcessingSequence[TokenNames.LinkStart].Peek() == 1 && word[i] == '<')
                    {
                        characterProcessingSequence[TokenNames.LinkStart].Dequeue();
                        ManagePointerStack(word, pointerToCurrentTokenStack, TokenNames.LinkStart, readParagraphBuffer);
                    }
                    else if (characterProcessingSequence[TokenNames.LinkEnd].Count > 0 && characterProcessingSequence[TokenNames.LinkEnd].Peek() == 1 && word[i] == '>')
                    {
                        characterProcessingSequence[TokenNames.LinkEnd].Dequeue();
                        ManagePointerStack(word, pointerToCurrentTokenStack, TokenNames.LinkEnd, readParagraphBuffer);
                    }
                    else if (word[i] != '\\' || word[i] == '\\' && characterProcessingSequence[TokenNames.Escaping].Count > 0 && characterProcessingSequence[TokenNames.Escaping].Dequeue() == 0)
                    {
                        readParagraphBuffer.AddSymbol(word[i]);
                    }
                }
                if (readParagraphBuffer.Buffer != "")
                {
                    pointerToCurrentTokenStack.Peek().Children.Add(new TextToken(readParagraphBuffer.Buffer));
                }
            }
        }

        return mainToken;
    }
    void ManagePointerStack(string markdownTextParagraph, Stack<BaseMarkdownToken> pointerToCurrentTokenStack, TokenNames tokenName, StringBuffer readParagraphBuffer)
    {
        if (tokenName == TokenNames.Bold)
        {
            if (pointerToCurrentTokenStack.Peek().TokenName == TokenNames.Bold) // Закрытие токена
            {
                var tempToken = new TextToken(readParagraphBuffer.Buffer);
                readParagraphBuffer.Clear();
                ((BoldToken)pointerToCurrentTokenStack.Peek()).ChangeStatus(TagEnum.Close);
                pointerToCurrentTokenStack.Pop().Children.Add(tempToken);
            }
            else 
            {
                if (GetElementOfStackByIndex(pointerToCurrentTokenStack, 1) != null && GetElementOfStackByIndex(pointerToCurrentTokenStack, 1).TokenName == TokenNames.Bold) //Когда предпредыдущий тег был __
                {
                    pointerToCurrentTokenStack.Pop();
                    readParagraphBuffer.AddSymbolToStartingString('_');
                }

                if (readParagraphBuffer.Buffer.Length > 0)
                {
                    var tempToken1 = new TextToken(readParagraphBuffer.Buffer);
                    readParagraphBuffer.Clear();
                    pointerToCurrentTokenStack.Peek().Children.Add(tempToken1);
                }

                var tempToken = new BoldToken(TagEnum.Open);
                pointerToCurrentTokenStack.Peek().Children.Add(tempToken);
                pointerToCurrentTokenStack.Push(tempToken);
            }
        }
        else if (tokenName == TokenNames.Italics)
        {
            if (pointerToCurrentTokenStack.Peek().TokenName == TokenNames.Italics) 
            {
                var tempToken = new TextToken(readParagraphBuffer.Buffer);
                readParagraphBuffer.Clear();
                ((ItalicsToken)pointerToCurrentTokenStack.Peek()).ChangeStatus(TagEnum.Close);
                pointerToCurrentTokenStack.Pop().Children.Add(tempToken);
            }
            else
            {
                if (readParagraphBuffer.Buffer.Length > 0)
                {
                    var tempToken1 = new TextToken(readParagraphBuffer.Buffer);
                    readParagraphBuffer.Clear();
                    pointerToCurrentTokenStack.Peek().Children.Add(tempToken1);
                }
                var tempToken = new ItalicsToken(TagEnum.Open);
                pointerToCurrentTokenStack.Peek().Children.Add(tempToken);
                pointerToCurrentTokenStack.Push(tempToken);
            }
        }
        else if (tokenName == TokenNames.LinkStart)
        {
            if (readParagraphBuffer.Buffer.Length > 0)
            {
                var tempToken1 = new TextToken(readParagraphBuffer.Buffer);
                readParagraphBuffer.Clear();
                pointerToCurrentTokenStack.Peek().Children.Add(tempToken1);
            }

            var tempToken = new LinkToken(TokenNames.LinkStart);
            pointerToCurrentTokenStack.Peek().Children.Add(tempToken);
            pointerToCurrentTokenStack.Push(tempToken);
        }
        else if (tokenName == TokenNames.LinkEnd)
        {
            if (pointerToCurrentTokenStack.Peek().TokenName == TokenNames.LinkStart)
            {
                var tempToken = new TextToken(readParagraphBuffer.Buffer);
                readParagraphBuffer.Clear();
                pointerToCurrentTokenStack.Pop().Children.Add(tempToken);
            }
        }
    }
    Dictionary<TokenNames, Queue<int>> GetCharacterProcessingSequence(string s)
    {
        Dictionary<TokenNames, Queue<int>> characterProcessingSequence = new Dictionary<TokenNames, Queue<int>>
        { 
            {TokenNames.Bold, new Queue<int>()},
            {TokenNames.Italics, new Queue<int>()}, 
            {TokenNames.LinkStart, new Queue<int>()}, 
            {TokenNames.LinkEnd, new Queue<int>()},
            {TokenNames.Escaping, new Queue<int>()}
        };
        Dictionary<TokenNames, int> tagsFound = new Dictionary<TokenNames, int> { { TokenNames.Bold, 0 }, { TokenNames.Italics, 0 }, { TokenNames.LinkStart, 0 }, { TokenNames.LinkEnd, 0 }, { TokenNames.Escaping, 0 } };
        string lastTag = "";
        int consecutiveEscapeCharactersCount = 0;
        for (int i = 0; i < s.Length; i++)
        {
            if (s[i] == '\\')
            {
                if (i+1 <= s.Length - 1 && !allowedTags.Contains(s[i + 1].ToString()))
                {
                    characterProcessingSequence[TokenNames.Escaping].Enqueue(0);
                }
                else
                {
                    if (consecutiveEscapeCharactersCount % 2 != 0)
                    {
                        characterProcessingSequence[TokenNames.Escaping].Enqueue(0);
                    }
                    else
                    {
                        characterProcessingSequence[TokenNames.Escaping].Enqueue(1);
                    }
                }
                consecutiveEscapeCharactersCount++;
            }
            else if (!(lastTag == "_" && tagsFound[TokenNames.Italics] % 2 != 0) && lastTag != "<" && ((i == 0 || !isEscapingSupported) || (isEscapingSupported && i > 0 && !(s[i - 1] == '\\' && consecutiveEscapeCharactersCount%2!=0))) && i + 1 < s.Length && s[i] == '_' && s[i + 1] == '_')
            {
                characterProcessingSequence[TokenNames.Bold].Enqueue(1);
                lastTag = "__";
                i += 1;
            }
            else if (lastTag != "<" && s[i] == '_' && (!isEscapingSupported || i == 0 || i > 0 && s[i - 1] != '\\'))
            {
                lastTag = "_";
                tagsFound[TokenNames.Italics]++;
                characterProcessingSequence[TokenNames.Italics].Enqueue(1);
            }
            else if (s[i] == '_')
            {
                characterProcessingSequence[TokenNames.Italics].Enqueue(0);
            }
            else if (s[i] == '>' && (!isEscapingSupported || i == 0 || i > 0 && s[i - 1] != '\\'))
            {
                lastTag = ">";
                characterProcessingSequence[TokenNames.LinkStart].Enqueue(1);
            }
            else if (lastTag != "<" && s[i] == '<' && (!isEscapingSupported || i == 0 || i > 0 && s[i - 1] != '\\'))
            {
                lastTag = "<";
                characterProcessingSequence[TokenNames.LinkEnd].Enqueue(1);
            }
            else if (s[i] == '<')
            {
                characterProcessingSequence[TokenNames.LinkStart].Enqueue(0);
            }
            if (s[i] != '\\')
            {
                consecutiveEscapeCharactersCount = 0;
            }
            if (i + 1 < s.Length && s[i] == '_' && s[i + 1] == '_')
            {
                characterProcessingSequence[TokenNames.Bold].Enqueue(0);
            }
        }

        return characterProcessingSequence;
        
    }

    T? GetElementOfStackByIndex<T>(Stack<T> stack, int index)
    {
        List<T> extractedElementsOfStack = new List<T>();
        if (index >= 0 && stack.Count > 0)
        {
            for (int i = 0; i < stack.Count; i++)
            {
                T stackElement = stack.Pop();
                extractedElementsOfStack.Add(stackElement);
                if (i == index)
                {
                    for (int j = extractedElementsOfStack.Count - 1; j >= 0; j--)
                    {
                        stack.Push(extractedElementsOfStack[j]);
                    }
                    return stackElement;
                }
            }
        }
        for (int j = extractedElementsOfStack.Count - 1; j >= 0; j--)
        {
            stack.Push(extractedElementsOfStack[j]);
        }
        return default;
    }
    
}
