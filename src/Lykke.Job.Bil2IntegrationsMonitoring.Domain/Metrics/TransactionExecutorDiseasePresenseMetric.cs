namespace Lykke.Job.Bil2IntegrationsMonitoring.Domain.Metrics
{
    public class TransactionExecutorDiseasePresenseMetric : MetricBase
    {
        protected static readonly string MetricName = "transaction_executor_disease_presence";

        public TransactionExecutorDiseasePresenseMetric(string integrationName) : base(integrationName)
        {
            Name = MetricName;
        }
    }
}
