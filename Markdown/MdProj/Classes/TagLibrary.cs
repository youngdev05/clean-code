using Markdown.Classes.Abstract_Classes;
using Markdown.Classes.Tags;

namespace Markdown.Classes;

public static class TagLibrary
{
    public static readonly string[] Tags = ["__", "_"];

    public static TagToken GetTagToken(string mdTag)
    {
        return mdTag switch
        {
            "######" => new Header(6),
            "#####"  => new Header(5),
            "####"   => new Header(4),
            "###"    => new Header(3),
            "##"     => new Header(2),
            "#"      => new Header(1),
            "__"     => new Bold(),
            "_"      => new Italic(),
            _        => throw new Exception($"Unknown tag: {mdTag}")
        };
    }
}