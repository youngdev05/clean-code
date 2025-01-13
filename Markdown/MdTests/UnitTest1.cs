

namespace MdTests;

public class MarkdownProcessorTests
{
    [Fact]
    public void SingleUnderscore_AppliesItalics()
    {
        var processor = new MdClass();
        string input = "Текст с _выделением_ через одиночное подчеркивание.";
        string expected = "<p>Текст с <em>выделением</em> через одиночное подчеркивание.</p>";
        Assert.Equal(expected, processor.RenderHtml(input));
    }

    [Fact]
    public void DoubleUnderscore_AppliesBold()
    {
        var processor = new Md();
        string input = "__Жирный текст__ должен быть выделен.";
        string expected = "<p><strong>Жирный текст</strong> должен быть выделен.</p>";
        Assert.Equal(expected, processor.RenderHtml(input));
    }
    
    [Fact]
    public void NestedItalicsInsideBold_WorksCorrectly()
    {
        var processor = new Md();
        string input = "Тут __жирный и _курсив внутри_ работает__ правильно.";
        string expected = "<p>Тут <strong>жирный и <em>курсив внутри</em> работает</strong> правильно.</p>";
        Assert.Equal(expected, processor.RenderHtml(input));
    }

    [Fact]
    public void ItalicsInsideBoldNotReversed()
    {
        var processor = new Md();
        string input = "_Курсив __не должен__ включать жирный_ внутри.";
        string expected = "<p><em>Курсив __не должен__ включать жирный</em> внутри.</p>";
        Assert.Equal(expected, processor.RenderHtml(input));
    }

    [Fact]
    public void Numbers_DoNotAffectFormatting()
    {
        var processor = new Md();
        string input = "В номере _12_34 не должно быть форматирования.";
        string expected = "<p>В номере _12_34 не должно быть форматирования.</p>";
        Assert.Equal(expected, processor.RenderHtml(input));
    }

    [Fact]
    public void UnderscoresInsideWords_AreNotFormatted()
    {
        var processor = new Md();
        string input = "Раз_рыв_ слова не считается выделением.";
        string expected = "<p>Раз_рыв_ слова не считается выделением.</p>";
        Assert.Equal(expected, processor.RenderHtml(input));
    }

    [Fact]
    public void UnmatchedUnderscores_StayUnchanged()
    {
        var processor = new Md();
        string input = "__Непарное подчеркивание_ не считается выделением.";
        string expected = "<p>__Непарное подчеркивание_ не считается выделением.</p>";
        Assert.Equal(expected, processor.RenderHtml(input));
    }

    [Fact]
    public void WhitespacePreventsFormatting()
    {
        var processor = new Md();
        string input = "Символы _ с пробелами _ не форматируются.";
        string expected = "<p>Символы _ с пробелами _ не форматируются.</p>";
        Assert.Equal(expected, processor.RenderHtml(input));
    }

    [Fact]
    public void OverlappingTags_DisablesFormatting()
    {
        var processor = new Md();
        string input = "Пересечение __вложенных _тегов__ приводит_ к обычному тексту.";
        string expected = "<p>Пересечение __вложенных _тегов__ приводит_ к обычному тексту.</p>";
        Assert.Equal(expected, processor.RenderHtml(input));
    }

    [Fact]
    public void EmptyTags_AreNotFormatted()
    {
        var processor = new Md();
        string input = "Пустые __ __ подчерки остаются как есть.";
        string expected = "<p>Пустые __ __ подчерки остаются как есть.</p>";
        Assert.Equal(expected, processor.RenderHtml(input));
    }

    [Fact]
    public void Headings_AreRenderedCorrectly()
    {
        var processor = new Md();
        string input = "# Заголовок __с _разными_ стилями__";
        string expected = "<h1>Заголовок <strong>с <em>разными</em> стилями</strong></h1>";
        Assert.Equal(expected, processor.RenderHtml(input));
    }
    
    [Fact]
    public void ParagraphsAndHeadings_RenderProperly()
    {
        var processor = new Md();
        string input = "Один абзац.

Второй абзац.

# Первый заголовок
## Второй заголовок
### Третий заголовок";
        string expected = "<p>Один абзац.</p><p>Второй абзац.</p><h1>Первый заголовок</h1><h2>Второй заголовок</h2><h3>Третий заголовок</h3>";
        Assert.Equal(expected, processor.RenderHtml(input));
    }

    [Fact]
    public void OrderedLists_RenderCorrectly()
    {
        var processor = new Md();
        string input = "1. Первый элемент
    1. Вложенный уровень
        1. Глубже
    2. Второй элемент второго уровня";
        string expected = "<ol><li>Первый элемент</li><ol><li>Вложенный уровень</li><ol><li>Глубже</li></ol><li>Второй элемент второго уровня</li></ol>";
        Assert.Equal(expected, processor.RenderHtml(input));
    }
}
