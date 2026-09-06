using System.Text;
using Aspose.BarCode.Generation;

namespace DataMatrix.Api.Generators;

/// <summary>
/// Генератор DataMatrix
/// </summary>
public static class DataMatrixGenerator
{
    /// <summary>
    /// Генерирует PNG-изображение DataMatrix на основе имени автора и случайного кода.
    /// ВНИМАНИЕ! Библиотека подключенная для генерации не предназначена для коммерческого использования 
    /// </summary>
    /// <param name="author">Имя автора, которое будет зашито в код.</param>
    /// <param name="code">Код</param>
    /// <returns>Массив байтов изображения в формате PNG.</returns>
    public static byte[] GenerateRandom(string author, string code)
    {
        StringBuilder builder = new($"Author:{author}\nCode:");
        builder.Append(code);
        
        using BarcodeGenerator generator = new(EncodeTypes.DataMatrix, builder.ToString());
        using MemoryStream memoryStream = new ();
        generator.Save(memoryStream, BarCodeImageFormat.Png);
        return memoryStream.ToArray();
    }
}