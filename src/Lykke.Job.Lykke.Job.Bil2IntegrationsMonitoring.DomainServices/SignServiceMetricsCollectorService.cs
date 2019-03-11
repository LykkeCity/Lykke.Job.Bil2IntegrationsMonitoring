using System.Threading.Tasks;
using Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.Domain.Services;

namespace Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.DomainServices
{
    public class SignServiceMetricsCollectorService
    {
        private readonly IMetricPublisher _metricPublisher;

        public SignServiceMetricsCollectorService(IMetricPublisher metricPublisher)
        {
            _metricPublisher = metricPublisher;
        }

        public async Task ReadIsAliveAsync()
        {

        }
    }
}
