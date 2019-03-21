namespace Lykke.Job.Bil2IntegrationsMonitoring.Domain.Metrics
{
    public class TimeFromLastBlockUpdateMetric : MetricBase
    {
        protected static readonly string MetricName = "time_from_last_block_update_seconds";

        public TimeFromLastBlockUpdateMetric(string integrationName) : base(integrationName)
        {
            Name = MetricName;
        }
    }
}
