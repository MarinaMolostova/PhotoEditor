using System.Text.Json;
using System.Text.Json.Serialization;
using PhotoEditor.Core.Interfaces;
using PhotoEditor.Core.Models;

namespace PhotoEditor.Core.Services;
// Класс для сохранения и загрузки проекта в формате JSON
public class JsonSessionSerializer : ISessionSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
    // Сохраняет весь проект (все слои и их данные) в JSON-файл.
    public void Save(PhotoDocument document, string filePath)
    {
        if (document == null) throw new ArgumentNullException(nameof(document));
        var json = JsonSerializer.Serialize(document, Options);
        File.WriteAllText(filePath, json);
    }
    // Загружает проект из JSON-файла.
    public PhotoDocument Load(string filePath)
    {
        if (!File.Exists(filePath)) throw new FileNotFoundException("Session file not found", filePath);
        var json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<PhotoDocument>(json, Options)
            ?? throw new InvalidOperationException("Failed to deserialize session");
    }
}