namespace DataMatrix.Web.Models.Settings
{
    public class CleanupWorkerSettings
    {
        /// <summary>
        /// Делей между запусками работы воркера
        /// </summary>
        public TimeSpan DoWorkDelay { get; set; }

        /// <summary>
        /// Сколько прошло времени, с момента его обновления, для его удаления.
        /// </summary>
        public TimeSpan FileLastUpdate { get; set; }
    }
}
