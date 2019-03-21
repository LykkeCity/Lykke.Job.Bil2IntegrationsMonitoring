using System.Collections.Generic;
using Lykke.Job.Bil2IntegrationsMonitoring.Settings.JobSettings;

namespace Lykke.Job.Bil2IntegrationsMonitoring.Services
{
    public class BlockchainIntegrationResolver
    {
        private readonly List<string> _integrationsList;

        public BlockchainIntegrationResolver(IEnumerable<BlockchainIntegration> blockchainIntegrations)
        {
            _integrationsList = new List<string>();

            foreach (var integration in blockchainIntegrations)
            {
                _integrationsList.Add(integration.Name);
            }
        }

        public IEnumerable<string> GetAllIntegrationNames()
        {
            return _integrationsList;
        }
    }
}
