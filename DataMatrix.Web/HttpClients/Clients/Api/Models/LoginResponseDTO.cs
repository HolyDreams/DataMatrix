using DataMatrix.Web.Enums;

namespace DataMatrix.Web.HttpClients.Clients.Api.Models
{
    public class LoginResponseDTO : BaseResponseDTO
    {
        public int? UserId { get; set; }

        public List<Role>? Roles { get; set; }
    }
}
