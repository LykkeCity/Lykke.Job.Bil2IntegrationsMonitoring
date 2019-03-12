namespace Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.Domain.Metrics
{
    public class TransactionExecutorIntegrationInfoResponseTimeMetric : MetricBase
    {
        protected static readonly string MetricName = "transaction_executor_integration_info_response_seconds";

        public TransactionExecutorIntegrationInfoResponseTimeMetric(string integrationName) : base(integrationName)
        {
            Name = MetricName;
        }
    }
}
