﻿using MdProcessor.Abstract_Classes;
using MdProcessor.Enums;

namespace MdProcessor.Tags;

public class Paragraph : TagToken
{
    public override string HtmlTag => "p";
    public override string MarkdownTag => "paragraph";
    public override TagType Type => TagType.Paragraph;
    public override TagPosition Position { get; set; }
    public override TagToken? Pair { get; set; }

    public override object Clone() => new Paragraph()
    {
        Pair = (TagToken?)Pair?.Clone(),
    };
}