namespace DataMatrix.Web.Models.Settings
{
    public class AppSettings
    {
        public required ApiHttpClientSettings ApiHttpClientSettings { get; set; }

        public required FileSettings FileSettings { get; set; }

        public required CleanupWorkerSettings CleanupWorkerSettings { get; set; }
    }
}
