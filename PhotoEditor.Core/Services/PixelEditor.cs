using System.Drawing;
using PhotoEditor.Core.Interfaces;

namespace PhotoEditor.Core.Services;
// Класс для изменения цвета одного конкретного пикселя.
public class PixelEditor : IPixelEditor
{
    public Bitmap SetPixel(Bitmap source, int x, int y, Color color)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        // Проверяем, не вышли ли координаты за границы картинки
        if (x < 0 || y < 0 || x >= source.Width || y >= source.Height)
            throw new ArgumentOutOfRangeException("Pixel coordinates out of bounds");

        var result = new Bitmap(source);
        result.SetPixel(x, y, color);
        return result;
    }
}