using System.Drawing;
using System.Drawing.Imaging;
using PhotoEditor.Core.Models;
using PhotoEditor.Core.Services;

namespace PhotoEditor.ConsoleUI;

internal static class Program
{
    private static void Main()
    {
        Console.InputEncoding = System.Text.Encoding.UTF8;
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var editor = new PhotoProcessor();
        var textWriter = new CaptionWriter();
        var pixelPainter = new PixelEditor();
        var gridComposer = new CollageComposer();
        var serializer = new JsonSessionSerializer();

        const string sessionPath = "my_project.json";

        var doc = new PhotoDocument();
        Bitmap? currentImage = null;
        int selectedIndex = -1; // Индекс текущего выбранного фото в списке слоев

        Console.WriteLine("ФОТОРЕДАКТОР");
        while (true)
        {
            Console.WriteLine();

            if (currentImage == null)
                Console.WriteLine("Проект пуст. Текущее фото не выбрано.");
            else
                Console.WriteLine($"Фотографий в проекте: {doc.Layers.Count}. Текущее: {doc.Layers[selectedIndex].Name} ({currentImage.Width}x{currentImage.Height})");

            Console.WriteLine("1. Добавить фото из файла");
            Console.WriteLine("2. Список и выбор фото");
            Console.WriteLine("3. Сведения о фото");
            Console.WriteLine("4. Обрезать фото");
            Console.WriteLine("5. Повернуть фото");
            Console.WriteLine("6. Добавить текст");
            Console.WriteLine("7. Изменить цвет одного пикселя");
            Console.WriteLine("8. Применить фильтр (яркость, ч/б, RGB)");
            Console.WriteLine("9. Создать коллаж");
            Console.WriteLine("10. Экспортировать фото");
            Console.WriteLine("11. Сохранить проект");
            Console.WriteLine("12. Загрузить проект");
            Console.WriteLine("13. Удалить выбранное фото");
            Console.WriteLine("0. Сохранить и выйти");
            Console.Write("Ваш выбор: ");

            string? choice = Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        Console.Write("Путь к фото: ");
                        string path = Console.ReadLine()?.Replace("\0", "").Replace("\"", "").Trim() ?? "";
                        if (!File.Exists(path)) throw new FileNotFoundException("Файл не найден.");

                        var newBmp = new Bitmap(path);
                        doc.Layers.Add(new Layer(Path.GetFileName(path), newBmp));
                        selectedIndex = doc.Layers.Count - 1;
                        currentImage = newBmp;
                        Console.WriteLine($" Фото '{Path.GetFileName(path)}' добавлено и выбрано.");
                        break;

                    case "2":
                        if (doc.Layers.Count == 0) { Console.WriteLine("Список пуст."); break; }
                        for (int i = 0; i < doc.Layers.Count; i++)
                        {
                            string mark = (i == selectedIndex) ? "[выбрано] " : "";
                            var bmp = doc.Layers[i].ToBitmap();
                            Console.WriteLine($"{i + 1}. {mark}{doc.Layers[i].Name} — {bmp.Width}x{bmp.Height}");
                            bmp.Dispose();
                        }
                        Console.Write("Номер фото (0 – назад): ");
                        if (int.TryParse(Console.ReadLine(), out int idx) && idx > 0 && idx <= doc.Layers.Count)
                        {
                            selectedIndex = idx - 1;
                            currentImage = doc.Layers[selectedIndex].ToBitmap();
                            Console.WriteLine($"Выбрано: {doc.Layers[selectedIndex].Name}");
                        }
                        break;

                    case "3":
                        if (currentImage == null) throw new InvalidOperationException("Сначала выберите фото.");
                        Console.WriteLine($"\nНазвание: {doc.Layers[selectedIndex].Name}");
                        Console.WriteLine($"Размер: {currentImage.Width} x {currentImage.Height} пикселей");
                        Console.WriteLine($"Данные PNG: {doc.Layers[selectedIndex].ImageData.Length:N0} байт");
                        break;

                    case "4":
                        if (currentImage == null) throw new InvalidOperationException("Сначала выберите фото.");
                        Console.WriteLine($"Размер: {currentImage.Width} × {currentImage.Height}. Координаты от нуля, левый верхний угол — (0, 0).");
                        Console.Write("X: "); int cx = int.Parse(Console.ReadLine()!);
                        Console.Write("Y: "); int cy = int.Parse(Console.ReadLine()!);
                        Console.Write("Ширина: "); int cw = int.Parse(Console.ReadLine()!);
                        Console.Write("Высота: "); int ch = int.Parse(Console.ReadLine()!);

                        currentImage = editor.Crop(currentImage, new Rectangle(cx, cy, cw, ch));
                        UpdateDocLayer(doc, selectedIndex, currentImage);
                        Console.WriteLine("Фото обрезано.");
                        break;

                    case "5":
                        if (currentImage == null) throw new InvalidOperationException("Сначала выберите фото.");
                        var rotated = editor.Rotate(currentImage, 90);
                        currentImage.Dispose();
                        currentImage = rotated;
                        UpdateDocLayer(doc, selectedIndex, currentImage);
                        Console.WriteLine("Фото повернуто на 90°!");
                        break;

                    case "6":
                        if (currentImage == null) throw new InvalidOperationException("Сначала выберите фото.");
                        Console.Write("Текст: "); string text = Console.ReadLine()!;
                        Console.Write("X: "); int tx = int.Parse(Console.ReadLine()!);
                        Console.Write("Y: "); int ty = int.Parse(Console.ReadLine()!);
                        Console.Write("Размер шрифта в пикселях: "); float fontSize = float.Parse(Console.ReadLine()!);
                        Console.Write("R (0..255): "); int rColor = int.Parse(Console.ReadLine()!);
                        Console.Write("G (0..255): "); int gColor = int.Parse(Console.ReadLine()!);
                        Console.Write("B (0..255): "); int bColor = int.Parse(Console.ReadLine()!);

                        using (var font = new Font("Arial", fontSize))
                        {
                            currentImage = textWriter.AddText(currentImage, text, new Point(tx, ty), font, Color.FromArgb(rColor, gColor, bColor));
                            UpdateDocLayer(doc, selectedIndex, currentImage);
                        }
                        Console.WriteLine("Надпись добавлена; часть за границей фото обрезается.");
                        break;

                    case "7":
                        if (currentImage == null) throw new InvalidOperationException("Сначала выберите фото.");
                        Console.Write("X: "); int px = int.Parse(Console.ReadLine()!);
                        Console.Write("Y: "); int py = int.Parse(Console.ReadLine()!);

                        var oldColor = currentImage.GetPixel(px, py);
                        Console.WriteLine($"Текущий цвет: R={oldColor.R}, G={oldColor.G}, B={oldColor.B}, A={oldColor.A}.");

                        Console.Write("R (0..255): "); int pr = int.Parse(Console.ReadLine()!);
                        Console.Write("G (0..255): "); int pg = int.Parse(Console.ReadLine()!);
                        Console.Write("B (0..255): "); int pb = int.Parse(Console.ReadLine()!);

                        currentImage = pixelPainter.SetPixel(currentImage, px, py, Color.FromArgb(pr, pg, pb));
                        UpdateDocLayer(doc, selectedIndex, currentImage);
                        Console.WriteLine("Пиксель изменён.");
                        break;

                    case "8":
                        if (currentImage == null) throw new InvalidOperationException("Сначала выберите фото.");
                        Console.WriteLine("1. Яркость\n2. Чёрно-белый\n3. Баланс RGB\n0. Назад");
                        Console.Write("Фильтр: ");
                        string fChoice = Console.ReadLine()!;

                        if (fChoice == "0") break;

                        if (fChoice == "1")
                        {
                            Console.Write("Изменение яркости (-100..100 %): ");
                            int brightPercent = int.Parse(Console.ReadLine()!);
                            // Переводим проценты в коэффициент (60 -> 1.6)
                            float factor = 1.0f + (brightPercent / 100.0f);
                            currentImage = editor.ApplyBrightness(currentImage, factor);
                        }
                        else if (fChoice == "2")
                        {
                            currentImage = editor.ApplyGrayscale(currentImage);
                        }
                        else if (fChoice == "3")
                        {
                            Console.Write("Сдвиг R (-255..255): "); int shiftR = int.Parse(Console.ReadLine()!);
                            Console.Write("Сдвиг G (-255..255): "); int shiftG = int.Parse(Console.ReadLine()!);
                            Console.Write("Сдвиг B (-255..255): "); int shiftB = int.Parse(Console.ReadLine()!);

                            // Переводим сдвиг -255..255 в коэффициент (например, 100 -> 1.39, -10 -> 0.96)
                            currentImage = editor.ApplyColorShift(currentImage, 1.0f + (shiftR / 255.0f), 1.0f + (shiftG / 255.0f), 1.0f + (shiftB / 255.0f));
                        }

                        if (fChoice == "1" || fChoice == "2" || fChoice == "3")
                        {
                            UpdateDocLayer(doc, selectedIndex, currentImage);
                            Console.WriteLine("Фильтр применён.");
                        }
                        break;

                    case "9":
                        if (doc.Layers.Count < 1) throw new InvalidOperationException("Для коллажа нужно хотя бы 1 фото.");

                        // Выводим список текущих фото в проекте
                        for (int i = 0; i < doc.Layers.Count; i++)
                        {
                            var layerBmp = doc.Layers[i].ToBitmap();
                            Console.WriteLine($"{i + 1}. {doc.Layers[i].Name} — {layerBmp.Width} × {layerBmp.Height}");
                            layerBmp.Dispose();
                        }

                        Console.Write("Номера фото через пробел в нужном порядке (повторы разрешены): ");
                        var numbers = (Console.ReadLine() ?? "").Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();

                        var sources = new List<byte[]>();
                        foreach (var num in numbers)
                        {
                            if (num < 1 || num > doc.Layers.Count) throw new ArgumentException($"Нет фото с номером {num}");
                            sources.Add(doc.Layers[num - 1].ImageData);
                        }

                        Console.WriteLine("Фото прилегают друг к другу. Края обрезаются по центру; последний ряд занимает всю ширину.");
                        Console.Write("Столбцов: "); int cols = int.Parse(Console.ReadLine()!);
                        Console.Write("Ширина одной ячейки, пикс.: "); int cellW = int.Parse(Console.ReadLine()!);
                        Console.Write("Высота одной ячейки, пикс.: "); int cellH = int.Parse(Console.ReadLine()!);
                        Console.Write("Название коллажа: "); string collageName = Console.ReadLine()!;

                        var collageBytes = gridComposer.Compose(sources, cols, cellW, cellH);
                        using (var ms = new MemoryStream(collageBytes))
                        {
                            var collageBmp = new Bitmap(ms);
                            doc.Layers.Add(new Layer(collageName, collageBmp));
                            selectedIndex = doc.Layers.Count - 1;
                            currentImage = collageBmp;
                        }
                        Console.WriteLine("Коллаж добавлен отдельным фото. Исходные фотографии сохранены.");
                        break;

                    case "10":
                        if (currentImage == null) throw new InvalidOperationException("Сначала выберите фото.");
                        Console.Write("Куда сохранить (.png, .jpg, .bmp): ");
                        string expPath = Console.ReadLine()?.Replace("\0", "").Replace("\"", "").Trim() ?? "result.png";
                        ImageFormat format = expPath.ToLower().EndsWith(".jpg") || expPath.ToLower().EndsWith(".jpeg") ? ImageFormat.Jpeg :
                                             expPath.ToLower().EndsWith(".bmp") ? ImageFormat.Bmp : ImageFormat.Png;
                        currentImage.Save(expPath, format);
                        Console.WriteLine($"Фото сохранено: {Path.GetFullPath(expPath)}");
                        break;

                    case "11":
                        serializer.Save(doc, sessionPath);
                        Console.WriteLine($"Проект сохранён: {Path.GetFullPath(sessionPath)}");
                        break;

                    case "12":
                        if (File.Exists(sessionPath))
                        {
                            doc = serializer.Load(sessionPath);
                            selectedIndex = doc.Layers.Count > 0 ? 0 : -1;
                            currentImage?.Dispose();
                            currentImage = selectedIndex >= 0 ? doc.Layers[0].ToBitmap() : null;
                            Console.WriteLine("Проект восстановлен.");
                        }
                        else Console.WriteLine("Файл не найден.");
                        break;

                    case "13":
                        if (currentImage == null) throw new InvalidOperationException("Сначала выберите фото.");
                        Console.Write("Удалить выбранное фото из проекта? (да (yes,y)/нет): ");
                        string confirm = Console.ReadLine()?.Replace("\0", "").Trim().ToLower() ?? "";

                        if (confirm == "да" || confirm == "yes" || confirm == "y")
                        {
                            string deletedName = doc.Layers[selectedIndex].Name;
                            doc.Layers.RemoveAt(selectedIndex);

                            if (doc.Layers.Count == 0)
                            {
                                selectedIndex = -1;
                            }
                            else if (selectedIndex >= doc.Layers.Count)
                            {
                                selectedIndex = doc.Layers.Count - 1;
                            }

                            currentImage?.Dispose();
                            currentImage = selectedIndex >= 0 ? doc.Layers[selectedIndex].ToBitmap() : null;

                            Console.WriteLine($"Фото '{deletedName}' удалено.");
                        }
                        else
                        {
                            Console.WriteLine("Удаление отменено.");
                        }
                        break;

                    case "0":
                        serializer.Save(doc, sessionPath);
                        Console.WriteLine("Сохранено. Выход.");
                        return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }
    // Вспомогательный метод для обновления текущего слоя в документе
    private static void UpdateDocLayer(PhotoDocument doc, int index, Bitmap newImage)
    {
        if (index >= 0 && index < doc.Layers.Count)
        {
            doc.Layers[index] = new Layer(doc.Layers[index].Name, newImage);
        }
    }
}