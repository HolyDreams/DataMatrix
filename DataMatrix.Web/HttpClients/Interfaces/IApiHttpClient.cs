using DataMatrix.Web.Enums;
using DataMatrix.Web.HttpClients.Clients.Api.Models;
using DataMatrix.Web.Models;

namespace DataMatrix.Web.HttpClients.Interfaces
{
    public interface IApiHttpClient
    {
        public Task<string> AuthAsync(string login, string password);
        
        public Task<string> RegisterAsync(string login, string password, params Role[] roles);

        public Task LogoutAsync(string cookie);

        public Task CreateRandomAsync(string cookie);

        public Task<List<CodeDTO>> GetAllAsync(string cookie);

        public Task<FileModel> Download(int id, string cookie);
    }
}
