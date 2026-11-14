using System.Drawing;

namespace PhotoEditor.Core.Interfaces;
// Интерфейс для точечного редактирования изображения. Позволяет изменять цвет одного конкретного пикселя.
public interface IPixelEditor
{
    Bitmap SetPixel(Bitmap source, int x, int y, Color color);
}