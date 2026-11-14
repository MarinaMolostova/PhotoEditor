using System.Drawing;
using PhotoEditor.Core.Models;
using PhotoEditor.Core.Services;
using Xunit;

namespace PhotoEditor.Tests;

public class JsonSessionSerializerTests
{
    [Fact]
    public void SaveAndLoad_RoundTrip_PreservesState()
    {
        // Создаем тестовое изображение
        using var bmp = new Bitmap(10, 10);
        using (var g = Graphics.FromImage(bmp)) g.Clear(Color.Green);

        // Создаем документ с одним слоем
        var doc = new PhotoDocument { Name = "Test Project" };
        doc.Layers.Add(new Layer("layer1", bmp, 5, 5));

        var path = Path.Combine(Path.GetTempPath(), $"test_session_{Guid.NewGuid()}.json");

        try
        {
            var ser = new JsonSessionSerializer();
            ser.Save(doc, path); // Сохраняем
            var loaded = ser.Load(path); // Загружаем

            // Проверяем, что всё восстановилось
            Assert.Equal("Test Project", loaded.Name);
            Assert.Single(loaded.Layers);
            Assert.Equal("layer1", loaded.Layers[0].Name);

            using var restored = loaded.Layers[0].ToBitmap();
            Assert.Equal(Color.Green.ToArgb(), restored.GetPixel(0, 0).ToArgb());
        }
        finally
        {
            // Удаляем временный файл
            if (File.Exists(path)) File.Delete(path);
        }
    }

    [Fact]
    public void Load_MissingFile_Throws()
    {
        var ser = new JsonSessionSerializer();
        Assert.Throws<FileNotFoundException>(() => ser.Load("no_such_file.json"));
    }
}