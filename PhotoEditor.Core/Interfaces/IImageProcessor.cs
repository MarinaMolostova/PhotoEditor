using System.Drawing;

namespace PhotoEditor.Core.Interfaces;
// Интерфейс для базовой обработки изображений. Определяет контракт для классов, которые умеют обрезать, поворачивать и применять цветовые фильтры к фотографиям.
public interface IImageProcessor
{
    Bitmap Crop(Bitmap source, Rectangle region); // Обрезает изображение по заданной прямоугольной области.
    Bitmap Rotate(Bitmap source, float angle); // Поворачивает изображение на заданный угол.
    Bitmap ApplyBrightness(Bitmap source, float factor); // Изменяет яркость изображения 
    Bitmap ApplyColorShift(Bitmap source, float r, float g, float b); // Изменяет баланс цветовых каналов (сдвиг RGB).
    Bitmap ApplyGrayscale(Bitmap source); // Черно-белый фильтр
}