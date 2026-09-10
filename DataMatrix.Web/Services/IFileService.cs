using DataMatrix.Web.Models;

namespace DataMatrix.Web.Services
{
    public interface IFileService
    {
        public Task<FileModel> GetFile(int id, string cookie);
    }
}
