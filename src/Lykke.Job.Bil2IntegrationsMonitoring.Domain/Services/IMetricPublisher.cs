using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Job.Bil2IntegrationsMonitoring.Domain.Metrics;

namespace Lykke.Job.Bil2IntegrationsMonitoring.Domain.Services
{
    public interface IMetricPublisher
    {
        Task PublishGaugeAsync(IMetric metric, params KeyValuePair<string, string>[] additionalLabels);
    }
}
