using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using PhotoEditor.Core.Interfaces;

namespace PhotoEditor.Core.Services;
// Класс, который собирает несколько фотографий в единый коллаж. Фото автоматически обрезаются по центру и заполняют ячейки без пробелов.
public class CollageComposer : ICollageComposer
{
    public byte[] Compose(IReadOnlyList<byte[]> sources, int columns, int cellWidth, int cellHeight)
    {
        if (sources == null || sources.Count == 0) throw new ArgumentException("Укажите фотографии для коллажа.");
        if (columns < 1 || columns > sources.Count) throw new ArgumentException("Укажите число столбцов от 1 до числа фото.");
        // Считаем количество строк (округляем в большую сторону для неполного последнего ряда)
        int rows = (sources.Count - 1) / columns + 1;
        int width = columns * cellWidth;
        int height = rows * cellHeight;

        using var result = new Bitmap(width, height, PixelFormat.Format32bppArgb);
        using (var graphics = Graphics.FromImage(result))
        using (var attributes = new ImageAttributes())
        {
            graphics.CompositingMode = CompositingMode.SourceCopy;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            attributes.SetWrapMode(WrapMode.TileFlipXY);

            for (int i = 0; i < sources.Count; i++)
            {
                using var ms = new MemoryStream(sources[i]);
                using var image = Image.FromStream(ms);
                int row = i / columns;
                int column = i % columns;
                int countInRow = Math.Min(columns, sources.Count - row * columns);

                int left = (int)((long)column * width / countInRow);
                int right = (int)((long)(column + 1) * width / countInRow);
                var destination = new Rectangle(left, row * cellHeight, right - left, cellHeight);
                // Масштабируем картинку, чтобы она заполнила ячейку без белых полей
                double scale = Math.Max((double)destination.Width / image.Width, (double)destination.Height / image.Height);
                float sourceWidth = (float)(destination.Width / scale);
                float sourceHeight = (float)(destination.Height / scale);
                float sourceX = (image.Width - sourceWidth) / 2; // Центрируем
                float sourceY = (image.Height - sourceHeight) / 2;

                graphics.DrawImage(image, destination, sourceX, sourceY, sourceWidth, sourceHeight, GraphicsUnit.Pixel, attributes);
            }
        }

        using var outStream = new MemoryStream();
        result.Save(outStream, ImageFormat.Png);
        return outStream.ToArray();
    }
}