using MdProj.Classes;

namespace MdTests;

public class MarkdownTests
{
    [Fact]
    public void Italics_EmphasizeTextBetweenSingleUnderscores()
    {
        var md = new MdClass(new Parser(), new MdRender(), new Resolver());
        string input = "Текст, _окруженный с двух сторон_ одинарными символами подчерка.";
        string expected = "<p>Текст, <em>окруженный с двух сторон</em> одинарными символами подчерка.</p>";
        Assert.Equal(expected, md.Process(input));
    }

    [Fact]
    public void Bold_EmphasizeTextBetweenDoubleUnderscores()
    {
        var md = new MdProcessor(new Parser(), new MdRender(), new Resolver());
        string input = "__Выделенный двумя символами текст__";
        string expected = "<p><strong>Выделенный двумя символами текст</strong></p>";
        Assert.Equal(expected, md.Process(input));
    }
    
    [Fact]
    public void NestedTags_SupportsItalicInsideBold()
    {
        var md = new MdProcessor(new Parser(), new MdRender(), new Resolver());
        string input = "Внутри __двойного выделения _одинарное_ тоже__ работает.";
        string expected = "<p>Внутри <strong>двойного выделения <em>одинарное</em> тоже</strong> работает.</p>";
        Assert.Equal(expected, md.Process(input));
    }

    [Fact]
    public void NestedTags_NotBoldInsideItalic()
    {
        var md = new MdProcessor(new Parser(), new MdRender(), new Resolver());
        string input = "Но не наоборот — внутри _одинарного __двойное__ не_ работает.";
        string expected = "<p>Но не наоборот — внутри <em>одинарного __двойное__ не</em> работает.</p>";
        Assert.Equal(expected, md.Process(input));
    }

    [Fact]
    public void Numbers_DoNotTriggerItalicFormatting()
    {
        var md = new MdProcessor(new Parser(), new MdRender(), new Resolver());
        string input = "Подчерки внутри текста c цифрами_12_3 не считаются выделением.";
        string expected = "<p>Подчерки внутри текста c цифрами_12_3 не считаются выделением.</p>";
        Assert.Equal(expected, md.Process(input));
    }

    [Fact]
    public void DifferentWords_AreNotItalicizedTogether()
    {
        var md = new MdProcessor(new Parser(), new MdRender(), new Resolver());
        string input = "В то же время выделение в ра_зных сл_овах не работает.";
        string expected = "<p>В то же время выделение в ра_зных сл_овах не работает.</p>";
        Assert.Equal(expected, md.Process(input));
    }

    [Fact]
    public void UnpairedUnderscores_DoNotTriggerFormatting()
    {
        var md = new MdProcessor(new Parser(), new MdRender(), new Resolver());
        string input = "__Непарные_ символы в рамках одного абзаца не считаются выделением.";
        string expected = "<p>__Непарные_ символы в рамках одного абзаца не считаются выделением.</p>";
        Assert.Equal(expected, md.Process(input));
    }

    [Fact]
    public void LeadingOrTrailingSpaces_PreventFormatting()
    {
        var md = new MdProcessor(new Parser(), new MdRender(), new Resolver());
        string input = "Эти_ подчерки_ не считаются выделением и остаются просто символами подчерка.";
        string expected = "<p>Эти_ подчерки_ не считаются выделением и остаются просто символами подчерка.</p>";
        Assert.Equal(expected, md.Process(input));
    }

    [Fact]
    public void CrossingTags_DisablesAllFormatting()
    {
        var md = new MdProcessor(new Parser(), new MdRender(), new Resolver());
        string input = "В случае __пересечения _двойных__ и одинарных_ подчерков ничего не выделяется.";
        string expected = "<p>В случае __пересечения _двойных__ и одинарных_ подчерков ничего не выделяется.</p>";
        Assert.Equal(expected, md.Process(input));
    }

    [Fact]
    public void EmptyStringInsideTags_DisablesFormatting()
    {
        var md = new MdProcessor(new Parser(), new MdRender(), new Resolver());
        string input = "Если внутри подчерков пустая строка ____, то они остаются символами подчерка.";
        string expected = "<p>Если внутри подчерков пустая строка ____, то они остаются символами подчерка.</p>";
        Assert.Equal(expected, md.Process(input));
    }

    [Fact]
    public void Headings_ApplyHeadingTag()
    {
        var md = new MdProcessor(new Parser(), new MdRender(), new Resolver());
        string input = "# Заголовок __с _разными_ символами__";
        string expected = "<h1>Заголовок <strong>с <em>разными</em> символами</strong></h1>";
        Assert.Equal(expected, md.Process(input));
    }
    
    [Fact]
    public void MultipleParagraphsAndHeadings_RenderCorrectly()
    {
        var md = new MdProcessor(new Parser(), new MdRender(), new Resolver());
        string input = "Первая строка абзаца.\n\nВторая строка абзаца.\n\n# Первый заголовок\n## Второй заголовок\n### Третий заголовок";
        string expected = "<p>Первая строка абзаца.</p><p>Вторая строка абзаца.</p><h1>Первый заголовок</h1><h2>Второй заголовок</h2><h3>Третий заголовок</h3>";
        Assert.Equal(expected, md.Process(input));
    }

    [Fact]
    public void ComplexOrderedList_RendersCorrectly()
    {
        var md = new MdProcessor(new Parser(), new MdRender(), new Resolver());
        string input = "1. Первый уровень\n    1. Второй уровень\n        1. Третий уровень\n    2. Второй элемент второго уровня";
        string expected = "<ol><li>Первый уровень</li><ol><li>Второй уровень</li><ol><li>Третий уровень</li></ol><li>Второй элемент второго уровня</li></ol>";
        Assert.Equal(expected, md.Process(input));
    }
}