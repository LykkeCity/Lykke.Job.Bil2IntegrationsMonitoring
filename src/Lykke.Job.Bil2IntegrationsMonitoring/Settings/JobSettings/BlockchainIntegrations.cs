using System.Collections.Generic;

namespace Lykke.Job.Bil2IntegrationsMonitoring.Settings.JobSettings
{
    public class BlockchainIntegrations
    {
        public IEnumerable<BlockchainIntegration> Integrations { get; set; }
    }

    public class BlockchainIntegration
    {
        public string Name { get; set; }

        public string TransactionExecutorUrl { get; set; }

        public string SignServiceUrl { get; set; }
    }
}
