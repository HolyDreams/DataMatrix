using DataMatrix.Web.HttpClients.Interfaces;
using DataMatrix.Web.Models;
using DataMatrix.Web.Models.Settings;

namespace DataMatrix.Web.Services
{
    public class FileService : IFileService
    {
        private readonly IApiHttpClient _apiHttpClient;
        private readonly FileSettings _settings;

        public FileService(IApiHttpClient apiHttpClient, FileSettings settings, ILogger<FileService> logger)
        {
            _apiHttpClient = apiHttpClient ?? throw new ArgumentNullException(nameof(apiHttpClient));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public async Task<FileModel> GetFile(int id, string cookie)
        {
            var path = Path.Combine(_settings.Directory, id + ".png");
            if (!_settings.AlwaysDownloadFile && TryGetFile(path, out var res) && res is not null)
                return res;

            return await _apiHttpClient.Download(id, cookie);
        }

        private bool TryGetFile(string path, out FileModel? model)
        {
            try
            {
                model = new FileModel(path);
                return true;
            }
            catch (FileNotFoundException)
            {
                model = null;
                return false;
            }
        }
    }
}
