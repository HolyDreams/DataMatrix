using System.Text.Json.Serialization;

namespace DataMatrix.Api.Models.Dtos;

/// <summary>
/// Модель кода для возврата в контроллере
/// </summary>
public class CodeDto
{
    public CodeDto() { }

    public CodeDto(Code code)
    {
        Id = code.Id;
        Value = code.Value;
        Author = code.User.UserName;
    }

    /// <summary>
    /// Идентификатор кода
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Код
    /// </summary>
    [JsonPropertyName("Code")]
    public string Value { get; set; }
    /// <summary>
    /// Автор кода
    /// </summary>
    public string? Author { get; set; }
}