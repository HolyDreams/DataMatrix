using DataMatrix.Web.Enums;
using DataMatrix.Web.HttpClients.Clients.Api.Models;
using DataMatrix.Web.Models;

namespace DataMatrix.Web.HttpClients.Interfaces
{
    public interface IApiHttpClient
    {
        public Task<ApiAuthResult> AuthAsync(string login, string password);
        
        public Task<ApiAuthResult> RegisterAsync(string login, string password, IEnumerable<Role> roles);

        public Task LogoutAsync();

        public Task CreateRandomAsync();

        public Task<List<CodeDTO>> GetAllAsync();

        public Task<FileModel> Download(int id);
    }
}
