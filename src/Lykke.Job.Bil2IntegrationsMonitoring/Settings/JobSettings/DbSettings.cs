using Lykke.SettingsReader.Attributes;

namespace Lykke.Job.Bil2IntegrationsMonitoring.Settings.JobSettings
{
    public class DbSettings
    {
        [AzureTableCheck]
        public string LogsConnString { get; set; }

        public string MongoConnString { get; set; }
    }
}
