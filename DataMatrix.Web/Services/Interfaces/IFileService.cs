using DataMatrix.Web.Models;

namespace DataMatrix.Web.Services.Interfaces
{
    public interface IFileService
    {
        public Task<FileModel> GetFile(int id);
    }
}
