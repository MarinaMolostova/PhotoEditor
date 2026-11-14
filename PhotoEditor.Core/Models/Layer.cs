using System.Drawing;

namespace PhotoEditor.Core.Models;
// Класс, представляющий один слой (фотографию) в документе.
public class Layer
{
    public string Name { get; set; } = "layer";
    public byte[] ImageData { get; set; } = Array.Empty<byte>();
    public int X { get; set; }
    public int Y { get; set; }

    public Layer() { }
    public Layer(string name, Bitmap bitmap, int x = 0, int y = 0) // Создает новый слой из объекта Bitmap.Автоматически конвертирует картинку в байты PNG.
    {
        Name = name;
        ImageData = BitmapConverter.ToPngBytes(bitmap);
        X = x;
        Y = y;
    }

    public Bitmap ToBitmap() => BitmapConverter.FromPngBytes(ImageData); // Превращает сохраненные байты PNG обратно в рабочее изображение Bitmap.
}