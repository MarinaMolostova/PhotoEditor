using System.Drawing;
using PhotoEditor.Core.Models;
using PhotoEditor.Core.Services;
using Xunit;

namespace PhotoEditor.Tests;

public class CollageComposerTests
{
    [Fact]
    public void Compose_TwoImages_ReturnsCorrectCanvasSize()
    {
        byte[] img1; byte[] img2;

        using (var bmp = new Bitmap(50, 50))
        { using var g = Graphics.FromImage(bmp); g.Clear(Color.Red); img1 = BitmapConverter.ToPngBytes(bmp); }

        using (var bmp = new Bitmap(50, 50))
        { using var g = Graphics.FromImage(bmp); g.Clear(Color.Blue); img2 = BitmapConverter.ToPngBytes(bmp); }

        var composer = new CollageComposer();
        // Делаем коллаж из 2 фото в 2 столбца, ячейки по 100х100
        byte[] resultBytes = composer.Compose(new[] { img1, img2 }, 2, 100, 100);

        using var result = BitmapConverter.FromPngBytes(resultBytes);

        // Ожидаем холст 200х100 (2 столбца * 100, 1 строка * 100)
        Assert.Equal(200, result.Width);
        Assert.Equal(100, result.Height);
    }
}