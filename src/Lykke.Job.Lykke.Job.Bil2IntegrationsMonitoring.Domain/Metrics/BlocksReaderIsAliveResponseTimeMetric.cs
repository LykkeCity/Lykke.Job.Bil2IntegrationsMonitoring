namespace Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.Domain.Metrics
{
    public class BlocksReaderIsAliveResponseTimeMetric : MetricBase
    {
        protected static readonly string MetricName = "blocks_reader_is_alive_response_seconds";

        public BlocksReaderIsAliveResponseTimeMetric(string integrationName) : base(integrationName)
        {
            Name = MetricName;
        }
    }
}
