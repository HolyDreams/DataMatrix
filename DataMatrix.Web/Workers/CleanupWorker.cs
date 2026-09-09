using DataMatrix.Web.Models.Settings;

namespace DataMatrix.Web.Workers
{
    public class CleanupWorker : BackgroundService
    {
        private readonly CleanupWorkerSettings _workerSettings;
        private readonly FileSettings _fileSettings;

        public CleanupWorker(CleanupWorkerSettings workerSettings, FileSettings fileSettings)
        {
            _fileSettings = fileSettings;
            _workerSettings = workerSettings;

            if (Directory.Exists(_fileSettings.Directory))
                Directory.CreateDirectory(_fileSettings.Directory);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var files = Directory.GetFiles(_fileSettings.Directory);
                foreach (var file in files)
                {
                    var fileInfo = new FileInfo(file);
                    if (fileInfo.LastWriteTime + _workerSettings.FileLastUpdate >= DateTime.Now)
                        File.Delete(file);
                }

                await Task.Delay(_workerSettings.DoWorkDelay, stoppingToken);
            }
        }
    }
}
