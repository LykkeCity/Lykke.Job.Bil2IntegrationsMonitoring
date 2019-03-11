using Lykke.SettingsReader.Attributes;

namespace Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.Settings.JobSettings
{
    public class RabbitMqSettings
    {
        [AmqpCheck]
        public string ConnectionString { get; set; }

        public string ExchangeName { get; set; }
    }
}
