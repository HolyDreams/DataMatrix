namespace DataMatrix.Web.Models.Settings
{
    public class FileSettings
    {
        /// <summary>
        /// Временная папка, в которой будут хранится файлы кодов.
        /// </summary>
        public required string Directory { get; set; }

        /// <summary>
        /// Всегда скачивать файл изображения или смотреть сначала на диске
        /// </summary>
        public required bool AlwaysDownloadFile { get; set; }
    }
}
