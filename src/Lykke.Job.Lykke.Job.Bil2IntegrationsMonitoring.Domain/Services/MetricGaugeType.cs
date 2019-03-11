namespace Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.Domain.Services
{
    public enum MetricGaugeType
    {
        [PrometheusNameTemplate("sign_service_is_alive_response_seconds")]
        SignServiceIsAliveResponseTime,

        [PrometheusNameTemplate("transaction_executor_is_alive_response_seconds")]
        TransactionExecutorIsAliveResponseTime,

        [PrometheusNameTemplate("blocks_reader_is_alive_response_seconds")]
        BlocksReaderIsAliveResponseTime,

        [PrometheusNameTemplate("transaction_executor_integration_info_response_seconds")]
        TransactionExecutorIntegrationInfoResponseTime,

        [PrometheusNameTemplate("time_from_last_block_update_seconds")]
        TimeFromLastBlockUpdate,

        [PrometheusNameTemplate("new_dependency_version")]
        NewDependencyVersionAvailable,

        //[PrometheusNameTemplate("presence_od_disease_bool")]
        //NewDependencyVersionAvailable,
    }
}
