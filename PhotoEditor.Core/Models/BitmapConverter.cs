using System.Drawing;
using System.Drawing.Imaging;

namespace PhotoEditor.Core.Models;
// Статический вспомогательный класс для конвертации изображений из объекта Bitmap в массив байтов (PNG) и обратно.
// Нужно для того, чтобы сохранять картинки внутри JSON-файла.
public static class BitmapConverter
{
    public static byte[] ToPngBytes(Bitmap bitmap)
    {
        using var ms = new MemoryStream();
        bitmap.Save(ms, ImageFormat.Png);
        return ms.ToArray();
    }

    public static Bitmap FromPngBytes(byte[] data)
    {
        using var ms = new MemoryStream(data);
        using var tmp = Image.FromStream(ms);
        return new Bitmap(tmp);
    }
}