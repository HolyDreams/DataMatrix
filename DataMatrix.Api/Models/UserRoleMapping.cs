namespace DataMatrix.Api.Models;

/// <summary>
/// Маппинг ролей
/// </summary>
public class UserRoleMapping
{
    public int UserId { get; set; }
    public int RoleId { get; set; }
    
    public User User { get; set; }
    public Role Role { get; set; }
}