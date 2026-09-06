namespace DataMatrix.Api.Models;

/// <summary>
/// Модель авторизации
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// Имя пользователя
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Пароль
    /// </summary>
    public string Password { get; set; }
}