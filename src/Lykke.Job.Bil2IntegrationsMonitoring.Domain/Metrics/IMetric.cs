namespace Lykke.Job.Bil2IntegrationsMonitoring.Domain.Metrics
{
    public interface IMetric
    {
        string IntegrationName { get; }

        string Name { get; }

        double Value { get; }
    }
}
