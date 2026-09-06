using Microsoft.AspNetCore.Identity;

namespace DataMatrix.Api.Models;

/// <summary>
/// Пользователь
/// </summary>
public class User : IdentityUser<int>
{
    /// <summary>
    /// Коды пользователя
    /// </summary>
    public ICollection<Code> Codes { get; set; } = [];
    /// <summary>
    /// Роли пользователя
    /// </summary>
    public ICollection<Role> Roles { get; set; } = [];
}