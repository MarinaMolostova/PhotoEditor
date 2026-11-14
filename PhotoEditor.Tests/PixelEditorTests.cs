using System.Drawing;
using PhotoEditor.Core.Services;
using Xunit;

namespace PhotoEditor.Tests;

public class PixelEditorTests
{
    [Fact]
    public void SetPixel_ChangesColor()
    {
        using var src = new Bitmap(5, 5);
        var editor = new PixelEditor();
        using var result = editor.SetPixel(src, 2, 2, Color.Red);

        Assert.Equal(Color.Red.ToArgb(), result.GetPixel(2, 2).ToArgb());
    }

    [Fact]
    public void SetPixel_OutOfBounds_Throws()
    {
        using var src = new Bitmap(5, 5);
        var editor = new PixelEditor();

        Assert.Throws<ArgumentOutOfRangeException>(() => editor.SetPixel(src, 10, 10, Color.Red));
    }
}