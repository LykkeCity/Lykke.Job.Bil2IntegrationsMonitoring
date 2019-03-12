using System;
using Lykke.Bil2.Client.TransactionsExecutor;
using Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.Domain.Metrics;
using Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.Domain.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.TransactionsExecutor.Responses;
using Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.Domain.Extensions;

namespace Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.DomainServices
{
    public class TransactionExecutorMetricsCollectorService : ITransactionExecutorMetricsCollectorService
    {
        private readonly IMetricPublisher _metricPublisher;
        private readonly ITransactionsExecutorApi _transactionsExecutorApi;
        private readonly string _integrationName;

        public TransactionExecutorMetricsCollectorService(
            string integrationName,
            IMetricPublisher metricPublisher,
            ITransactionsExecutorApi transactionsExecutorApi)
        {
            _integrationName = integrationName.UseLowercaseAndUnderscoreDelimeter();
            _metricPublisher = metricPublisher;
            _transactionsExecutorApi = transactionsExecutorApi;
        }

        public async Task MeasureIsAliveAsync()
        {
            var metric = new TransactionExecutorIsAliveResponseTimeMetric(_integrationName);
            var diseaseMetric = new TransactionExecutorDiseasePresenseMetric(_integrationName);
            TransactionsExecutorIsAliveResponse isAliveResponse = null;
            using (var timer = new MetricTimer(metric))
            {
                isAliveResponse = await _transactionsExecutorApi.GetIsAliveAsync();
            }

            diseaseMetric.Set(string.IsNullOrEmpty(isAliveResponse.Disease) ? 0 : 1);

            await _metricPublisher.PublishGaugeAsync(diseaseMetric);
            await _metricPublisher.PublishGaugeAsync(metric);
        }

        public async Task MeasureGetInfoAsync()
        {
            var metric = new TransactionExecutorIsAliveResponseTimeMetric(_integrationName);
            var timeFromLastBlockMetric = new TimeFromLastBlockUpdateMetric(_integrationName);
            IntegrationInfoResponse response = null;
            using (var timer = new MetricTimer(metric))
            {
                response = await _transactionsExecutorApi.GetIntegrationInfoAsync();
            }

            var diff = DateTime.UtcNow - response.Blockchain.LatestBlockMoment;
            timeFromLastBlockMetric.Set(diff.Seconds);

            await _metricPublisher.PublishGaugeAsync(metric);
            await _metricPublisher.PublishGaugeAsync(timeFromLastBlockMetric);
        }

        public async Task MeasureDependencyVersionsAsync()
        {
            List<NewDependencyVersionAvailableMetric> metrics = null;
            var response = await _transactionsExecutorApi.GetIntegrationInfoAsync();

            if (response?.Dependencies != null)
            {
                metrics = new List<NewDependencyVersionAvailableMetric>(response.Dependencies.Count);
                foreach (var dependency in response.Dependencies)
                {
                    var updateIsAvailable = dependency.Value.RunningVersion < dependency.Value.LatestAvailableVersion;
                    var metric = new NewDependencyVersionAvailableMetric(_integrationName, dependency.Key);
                    metric.Set(updateIsAvailable ? 1 : 0);
                    metrics.Add(metric);
                }
            }

            if (metrics != null && metrics.Any())
            {
                foreach (var metric in metrics)
                {
                    await _metricPublisher.PublishGaugeAsync(metric);
                }
            }
        }
    }
}
