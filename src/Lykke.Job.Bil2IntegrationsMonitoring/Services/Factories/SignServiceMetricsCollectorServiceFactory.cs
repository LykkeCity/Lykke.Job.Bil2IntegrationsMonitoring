using System.Threading.Tasks;
using Lykke.Bil2.Client.SignService.Services;
using Lykke.Job.Bil2IntegrationsMonitoring.Domain.Services;
using Lykke.Job.Bil2IntegrationsMonitoring.DomainServices;

namespace Lykke.Job.Bil2IntegrationsMonitoring.Services.Factories
{
    public class SignServiceMetricsCollectorServiceFactory : ISignServiceMetricsCollectorServiceFactory
    {
        private readonly IMetricPublisher _metricPublisher;
        private readonly ISignServiceApiProvider _signServiceApiProvider;

        public SignServiceMetricsCollectorServiceFactory(IMetricPublisher metricPublisher,
            ISignServiceApiProvider signServiceApiProvider)
        {
            _metricPublisher = metricPublisher;
            _signServiceApiProvider = signServiceApiProvider;
        }

        public async Task MeasureIsAliveAsync(string integrationName)
        {
            var measureService = new SignServiceMetricsCollectorService(integrationName, 
                _metricPublisher, 
                _signServiceApiProvider);

            await measureService.MeasureIsAliveAsync();
        }
    }
}
