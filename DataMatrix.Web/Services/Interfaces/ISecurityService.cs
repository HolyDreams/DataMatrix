using DataMatrix.Web.Enums;
using DataMatrix.Web.Models;

namespace DataMatrix.Web.Services.Interfaces
{
    public interface ISecurityService
    {
        Task<SecurityAuthResult> Auth(HttpContext context, string login, string password);

        Task<SecurityAuthResult> Register(HttpContext context, string login, string password, IEnumerable<Role> roles);

        Task Logout(HttpContext context);
    }
}
