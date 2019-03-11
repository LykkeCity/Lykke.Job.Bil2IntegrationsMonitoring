using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.Domain.Services
{
    public interface IMetricPublisher
    {
        Task PublishGaugeAsync(string integrationName,
            MetricGaugeType metricType,
            double metricValue,
            params KeyValuePair<string, string>[] additionalLabels);
    }
}
