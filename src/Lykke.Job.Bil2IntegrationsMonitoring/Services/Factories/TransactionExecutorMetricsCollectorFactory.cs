using System.Threading.Tasks;
using Lykke.Bil2.Client.TransactionsExecutor.Services;
using Lykke.Job.Bil2IntegrationsMonitoring.Domain.Services;
using Lykke.Job.Bil2IntegrationsMonitoring.DomainServices;

namespace Lykke.Job.Bil2IntegrationsMonitoring.Services.Factories
{
    public class TransactionExecutorMetricsCollectorFactory : ITransactionExecutorMetricsCollectorFactory
    {
        private readonly IMetricPublisher _metricPublisher;
        private readonly ITransactionsExecutorApiProvider _transactionsExecutorApiProvider;

        public TransactionExecutorMetricsCollectorFactory(IMetricPublisher metricPublisher,
            ITransactionsExecutorApiProvider transactionsExecutorApiProvider)
        {
            _metricPublisher = metricPublisher;
            _transactionsExecutorApiProvider = transactionsExecutorApiProvider;
        }

        public async Task MeasureIsAliveAsync(string integrationName)
        {
            var measureService = GetMetricsCollectorService(integrationName);

            await measureService.MeasureIsAliveAsync();
        }

        public async Task MeasureDependencyVersionsAsync(string integrationName)
        {
            var measureService = GetMetricsCollectorService(integrationName);

            await measureService.MeasureDependencyVersionsAsync();
        }

        public async Task MeasureGetInfoAsync(string integrationName)
        {
            var measureService = GetMetricsCollectorService(integrationName);

            await measureService.MeasureGetInfoAsync();
        }

        private TransactionExecutorMetricsCollectorService GetMetricsCollectorService(string integrationName)
        {
            var measureService = new TransactionExecutorMetricsCollectorService(integrationName,
                _metricPublisher,
                _transactionsExecutorApiProvider.GetApi(integrationName));

            return measureService;
        }
    }
}
