using System.Drawing;
using PhotoEditor.Core.Interfaces;

namespace PhotoEditor.Core.Services;
// Класс для нанесения текста на изображение
public class CaptionWriter : ITextRenderer
{
    // Добавляет текст на картинку в указанных координатах
    public Bitmap AddText(Bitmap source, string text, Point position, Font font, Color color)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (font == null) throw new ArgumentNullException(nameof(font));

        var result = new Bitmap(source); // Делаем копию, чтобы не испортить оригинал
        using var g = Graphics.FromImage(result);
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit; // Cглаживание для красивого текста
        using var brush = new SolidBrush(color);
        g.DrawString(text ?? "", font, brush, position);
        return result;
    }
}