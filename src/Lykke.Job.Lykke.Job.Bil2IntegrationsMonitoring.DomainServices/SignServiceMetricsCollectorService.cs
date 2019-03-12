using Lykke.Bil2.Client.SignService;
using Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.Domain.Extensions;
using Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.Domain.Metrics;
using Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.Domain.Services;
using System.Threading.Tasks;

namespace Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.DomainServices
{
    public class SignServiceMetricsCollectorService : ISignServiceMetricsCollectorService
    {
        private readonly IMetricPublisher _metricPublisher;
        private readonly ISignServiceApi _signServiceApi;
        private readonly string _integrationName;

        public SignServiceMetricsCollectorService(
            string integrationName,
            IMetricPublisher metricPublisher, 
            ISignServiceApi signServiceApi)
        {
            _integrationName = integrationName.UseLowercaseAndUnderscoreDelimeter();
            _metricPublisher = metricPublisher;
            _signServiceApi = signServiceApi;
        }

        public async Task MeasureIsAliveAsync()
        {
            var metric = new SignServiceIsAliveResponseTimeMetric(_integrationName);
            using (var timer = new MetricTimer(metric))
            {
                await _signServiceApi.GetIsAliveAsync();
            }

            await _metricPublisher.PublishGaugeAsync(metric);
        }
    }
}
