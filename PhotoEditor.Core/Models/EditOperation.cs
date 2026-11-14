namespace PhotoEditor.Core.Models;
// Класс, который хранит информацию о выполненной операции.
public class EditOperation
{
    public string Type { get; set; } = "";
    public Dictionary<string, string> Params { get; set; } = new();

    public EditOperation() { }
    public EditOperation(string type, Dictionary<string, string>? p = null)
    {
        Type = type;
        Params = p ?? new Dictionary<string, string>();
    }
}