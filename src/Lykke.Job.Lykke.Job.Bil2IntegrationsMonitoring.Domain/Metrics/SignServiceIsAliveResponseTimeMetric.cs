namespace Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.Domain.Metrics
{
    public class SignServiceIsAliveResponseTimeMetric : MetricBase
    {
        protected static readonly string MetricName = "sign_service_is_alive_response_seconds";

        public SignServiceIsAliveResponseTimeMetric(string integrationName) : base(integrationName)
        {
            Name = MetricName;
        }
    }
}
