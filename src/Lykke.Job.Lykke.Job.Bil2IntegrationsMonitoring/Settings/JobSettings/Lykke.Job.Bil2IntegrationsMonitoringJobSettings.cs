namespace Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.Settings.JobSettings
{
    public class MonitoringJobSettings
    {
        public DbSettings Db { get; set; }
        public AzureQueueSettings AzureQueue { get; set; }
        public RabbitMqSettings Rabbit { get; set; }
    }
}
