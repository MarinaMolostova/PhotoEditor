using System.Drawing;
using System.Drawing.Drawing2D;
using PhotoEditor.Core.Interfaces;

namespace PhotoEditor.Core.Services;
// Класс для базовой обработки изображений (обрезка, поворот, фильтры). Реализует интерфейс IImageProcessor.
public class PhotoProcessor : IImageProcessor
{
    public Bitmap Crop(Bitmap source, Rectangle region)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        // Проверка: если область выходит за границы, выбрасываем ошибку
        if (region.X < 0 || region.Y < 0 ||
            region.X + region.Width > source.Width ||
            region.Y + region.Height > source.Height)
        {
            throw new ArgumentException("Область обрезки выходит за пределы изображения.");
        }

        var result = new Bitmap(region.Width, region.Height);
        using var g = Graphics.FromImage(result);
        g.DrawImage(source, new Rectangle(0, 0, region.Width, region.Height), region, GraphicsUnit.Pixel);
        return result;
    }
    // Поворачивает изображение на 90 градусов по часовой стрелке. Размеры ширины и высоты меняются местами.
    public Bitmap Rotate(Bitmap source, float angle)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        var result = new Bitmap(source);
        result.RotateFlip(RotateFlipType.Rotate90FlipNone);
        return result;
    }
    // Изменяет яркость изображения 
    public Bitmap ApplyBrightness(Bitmap source, float factor)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        var result = new Bitmap(source.Width, source.Height);
        for (int y = 0; y < source.Height; y++)
        {
            for (int x = 0; x < source.Width; x++)
            {
                var c = source.GetPixel(x, y);
                result.SetPixel(x, y, Color.FromArgb(
                    c.A,
                    Clamp((int)(c.R * factor)),
                    Clamp((int)(c.G * factor)),
                    Clamp((int)(c.B * factor))));
            }
        }
        return result;
    }

    public Bitmap ApplyColorShift(Bitmap source, float r, float g, float b)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        var result = new Bitmap(source.Width, source.Height);
        for (int y = 0; y < source.Height; y++)
        {
            for (int x = 0; x < source.Width; x++)
            {
                var c = source.GetPixel(x, y);
                result.SetPixel(x, y, Color.FromArgb(
                    c.A,
                    Clamp((int)(c.R * r)),
                    Clamp((int)(c.G * g)),
                    Clamp((int)(c.B * b))));
            }
        }
        return result;
    }

    public Bitmap ApplyGrayscale(Bitmap source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        var result = new Bitmap(source.Width, source.Height);
        for (int y = 0; y < source.Height; y++)
        {
            for (int x = 0; x < source.Width; x++)
            {
                var c = source.GetPixel(x, y);
                int gray = (int)(c.R * 0.299 + c.G * 0.587 + c.B * 0.114);
                result.SetPixel(x, y, Color.FromArgb(c.A, gray, gray, gray));
            }
        }
        return result;
    }

    private static int Clamp(int v) => v < 0 ? 0 : v > 255 ? 255 : v;
}