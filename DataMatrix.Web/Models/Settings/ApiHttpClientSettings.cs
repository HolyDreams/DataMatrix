namespace DataMatrix.Web.Models.Settings
{
    public class ApiHttpClientSettings
    {
        /// <summary>
        /// Адрес до апи
        /// </summary>
        public required string BaseAddress { get; set; }

        /// <summary>
        /// Максимальное время на ожидание ответа
        /// </summary>
        public TimeSpan MaxTimeout { get; set; }
    }
}
