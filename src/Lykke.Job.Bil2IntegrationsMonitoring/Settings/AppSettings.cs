using Lykke.Job.Bil2IntegrationsMonitoring.Settings.JobSettings;
using Lykke.Sdk.Settings;

namespace Lykke.Job.Bil2IntegrationsMonitoring.Settings
{
    public class AppSettings : BaseAppSettings
    {
        public Bil2MonitoringJobSettings Bil2MonitoringJobSettings { get; set; }

        public BlockchainIntegrations BlockchainIntegrations { get; set; }
    }
}
