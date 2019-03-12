using Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.Domain.Metrics;

namespace Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.Tests
{
    internal class TestMetric : MetricBase
    {
        public TestMetric(string integrationName) : base(integrationName)
        {
            Name = "TestMetric";
        }
    }
}
