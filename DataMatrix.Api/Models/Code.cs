namespace DataMatrix.Api.Models;

/// <summary>
/// Код
/// </summary>
public class Code
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Значение
    /// </summary>
    public string Value { get; set; }
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public int UserId { get; set; }
    /// <summary>
    /// Пользователь
    /// </summary>
    public User User { get; set; } 
}