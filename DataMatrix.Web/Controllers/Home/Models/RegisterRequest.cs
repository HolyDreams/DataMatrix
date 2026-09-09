using DataMatrix.Web.Enums;

namespace DataMatrix.Web.Controllers.Home.Models
{
    public class RegisterRequest
    {
        public required string Login { get; set; }
        public required string Password { get; set; }
        public required IEnumerable<Role> Roles { get; set; }
    }
}
