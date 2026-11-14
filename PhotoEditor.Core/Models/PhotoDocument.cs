namespace PhotoEditor.Core.Models;
// Содержит список всех загруженных фотографий (слоев) и историю операций. Сохраняется в JSON при выборе пункта "Сохранить проект".
public class PhotoDocument
{
    public string Name { get; set; } = "Untitled";
    public List<Layer> Layers { get; set; } = new();
    public List<EditOperation> Operations { get; set; } = new();
}