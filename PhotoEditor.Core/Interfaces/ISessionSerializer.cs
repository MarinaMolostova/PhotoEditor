using PhotoEditor.Core.Models;

namespace PhotoEditor.Core.Interfaces;
// Интерфейс для сохранения и загрузки состояния проекта 
public interface ISessionSerializer
{
    void Save(PhotoDocument document, string filePath); // Сохраняет весь документ (все слои и их данные) в файл.
    PhotoDocument Load(string filePath); // Загружает документ из файла.
}