using System.Drawing;
using PhotoEditor.Core.Services;
using Xunit;

namespace PhotoEditor.Tests;

public class CaptionWriterTests
{
    [Fact]
    public void AddText_ReturnsSameSize()
    {
        using var src = new Bitmap(100, 50);
        var writer = new CaptionWriter();
        using var font = new Font("Arial", 12);
        using var result = writer.AddText(src, "Привет", new Point(5, 5), font, Color.Black);

        Assert.Equal(100, result.Width);
        Assert.Equal(50, result.Height);
    }

    [Fact]
    public void AddText_ChangesPixels()
    {
        using var src = new Bitmap(100, 50);
        using (var g = Graphics.FromImage(src)) g.Clear(Color.White); // Белый фон

        var writer = new CaptionWriter();
        using var font = new Font("Arial", 20, FontStyle.Bold);
        using var result = writer.AddText(src, "Test", new Point(10, 10), font, Color.Black);

        // Проверяем, что в области текста появились темные пиксели
        bool hasDarkPixels = false;
        for (int x = 10; x < 50; x++)
            for (int y = 10; y < 40; y++)
                if (result.GetPixel(x, y).R < 100) hasDarkPixels = true;

        Assert.True(hasDarkPixels, "Текст должен оставить темные пиксели.");
    }
}