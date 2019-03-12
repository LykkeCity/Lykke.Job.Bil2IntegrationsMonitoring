namespace Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.Domain.Metrics
{
    public class TransactionExecutorIsAliveResponseTimeMetric : MetricBase
    {
        protected static readonly string MetricName = "transaction_executor_is_alive_response_seconds";

        public TransactionExecutorIsAliveResponseTimeMetric(string integrationName) : base(integrationName)
        {
            Name = MetricName;
        }
    }
}
