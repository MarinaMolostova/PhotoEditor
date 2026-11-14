using System.Drawing;
using PhotoEditor.Core.Services;
using Xunit;

namespace PhotoEditor.Tests;

public class PhotoProcessorTests
{
    [Fact]
    public void Crop_ReturnsCorrectSize()
    {
        using var src = new Bitmap(10, 10);
        var processor = new PhotoProcessor();
        using var result = processor.Crop(src, new Rectangle(2, 2, 5, 5));

        Assert.Equal(5, result.Width);
        Assert.Equal(5, result.Height);
    }

    [Fact]
    public void Crop_OutOfBounds_Throws()
    {
        using var src = new Bitmap(10, 10);
        var processor = new PhotoProcessor();
        Assert.Throws<ArgumentException>(() => processor.Crop(src, new Rectangle(8, 8, 5, 5)));
    }

    [Fact]
    public void Rotate90_SwapsDimensions()
    {
        using var src = new Bitmap(10, 20);
        var processor = new PhotoProcessor();
        using var result = processor.Rotate(src, 90);

        Assert.Equal(20, result.Width);
        Assert.Equal(10, result.Height);
    }

    [Fact]
    public void ApplyBrightness_MakesImageBrighter()
    {
        using var src = new Bitmap(10, 10);
        using (var g = Graphics.FromImage(src)) g.Clear(Color.FromArgb(100, 100, 100));

        var processor = new PhotoProcessor();
        using var result = processor.ApplyBrightness(src, 1.5f); // Осветляем в 1.5 раза

        Assert.Equal(150, result.GetPixel(0, 0).R);
    }

    [Fact]
    public void ApplyGrayscale_MakesAllChannelsEqual()
    {
        using var src = new Bitmap(10, 10);
        using (var g = Graphics.FromImage(src)) g.Clear(Color.Red);

        var processor = new PhotoProcessor();
        using var result = processor.ApplyGrayscale(src);
        var pixel = result.GetPixel(0, 0);

        Assert.Equal(pixel.R, pixel.G);
        Assert.Equal(pixel.G, pixel.B);
    }

    [Fact]
    public void ApplyColorShift_ChangesChannels()
    {
        using var src = new Bitmap(10, 10);
        using (var g = Graphics.FromImage(src)) g.Clear(Color.FromArgb(100, 100, 100));

        var processor = new PhotoProcessor();
        using var result = processor.ApplyColorShift(src, 1.5f, 1.0f, 0.5f);
        var pixel = result.GetPixel(0, 0);

        Assert.Equal(150, pixel.R);
        Assert.Equal(100, pixel.G);
        Assert.Equal(50, pixel.B);
    }
}