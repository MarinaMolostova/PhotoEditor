using System.Drawing;

namespace PhotoEditor.Core.Interfaces;
// Интерфейс для нанесения текста на изображение.
public interface ITextRenderer
{
    Bitmap AddText(Bitmap source, string text, Point position, Font font, Color color);
}