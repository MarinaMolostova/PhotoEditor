namespace PhotoEditor.Core.Interfaces;
// Интерфейс для сборки коллажей из нескольких фотографий.
public interface ICollageComposer
{
    byte[] Compose(IReadOnlyList<byte[]> sources, int columns, int cellWidth, int cellHeight);
}