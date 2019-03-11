using Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.Settings.JobSettings;
using Lykke.Sdk.Settings;

namespace Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.Settings
{
    public class AppSettings : BaseAppSettings
    {
        public MonitoringJobSettings MonitoringJob { get; set; }
    }
}
