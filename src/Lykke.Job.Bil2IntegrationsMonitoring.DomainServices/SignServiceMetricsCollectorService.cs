using System.Threading.Tasks;
using Lykke.Bil2.Client.SignService;
using Lykke.Bil2.Client.SignService.Services;
using Lykke.Job.Bil2IntegrationsMonitoring.Domain.Extensions;
using Lykke.Job.Bil2IntegrationsMonitoring.Domain.Metrics;
using Lykke.Job.Bil2IntegrationsMonitoring.Domain.Services;

namespace Lykke.Job.Bil2IntegrationsMonitoring.DomainServices
{
    public class SignServiceMetricsCollectorService : ISignServiceMetricsCollectorService
    {
        private readonly IMetricPublisher _metricPublisher;
        private readonly ISignServiceApi _signServiceApi;
        private readonly string _integrationName;

        public SignServiceMetricsCollectorService(
            string integrationName,
            IMetricPublisher metricPublisher, 
            ISignServiceApiProvider signServiceApiProvider)
        {
            _integrationName = integrationName.UseLowercaseAndUnderscoreDelimeter();
            _metricPublisher = metricPublisher;
            _signServiceApi = signServiceApiProvider.GetApi(integrationName);
        }

        public async Task MeasureIsAliveAsync()
        {
            var metric = new SignServiceIsAliveResponseTimeMetric(_integrationName);

            await MetricTimer.MeasureSafelyAsync(async () => { await _signServiceApi.GetIsAliveAsync(); },
                metric);

            await _metricPublisher.PublishGaugeAsync(metric);
        }
    }
}
