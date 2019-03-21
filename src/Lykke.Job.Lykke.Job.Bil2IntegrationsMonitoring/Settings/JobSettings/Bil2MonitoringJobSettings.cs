using System;
using JetBrains.Annotations;

namespace Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.Settings.JobSettings
{
    public class Bil2MonitoringJobSettings
    {
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string PrometheusPushGatewayUrl { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public TimeSpan BlockchainIntegrationTimeout { get; set; }

        public DbSettings Db { get; set; }

        //public RabbitMqSettings Rabbit { get; set; }

        public BlockchainIntegrations BlockchainIntegrations { get; set; }
    }
}
