namespace DataMatrix.Api.Models;

/// <summary>
/// Запрос регистрации пользователя
/// </summary>
public class RegisterRequest
{
    /// <summary>
    /// Имя пользователя
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Пароль
    /// </summary>
    public string Password { get; set; }
    /// <summary>
    /// Роли
    /// </summary>
    public List<string> Roles { get; set; }
}