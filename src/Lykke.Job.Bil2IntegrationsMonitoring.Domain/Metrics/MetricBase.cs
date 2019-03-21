namespace Lykke.Job.Bil2IntegrationsMonitoring.Domain.Metrics
{
    public abstract class MetricBase : IMetric
    {
        public MetricBase(string integrationName)
        {
            IntegrationName = integrationName;
        }

        public string IntegrationName { get; protected set; }
        public string Name { get; protected set; }
        public double Value { get; protected set; }

        public void Set(double value)
        {
            Value = value;
        }
    }
}
