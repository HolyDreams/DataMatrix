using Microsoft.AspNetCore.Identity;

namespace DataMatrix.Api.Models;

/// <summary>
/// Роль
/// </summary>
public class Role : IdentityRole<int>
{
    /// <summary>
    /// Пользователи Роли
    /// </summary>
    public ICollection<User> Users { get; set; } = [];
}