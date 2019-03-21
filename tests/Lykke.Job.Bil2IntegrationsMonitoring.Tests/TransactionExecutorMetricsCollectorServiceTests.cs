using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Bil2.Client.TransactionsExecutor;
using Lykke.Bil2.Contract.TransactionsExecutor.Responses;
using Lykke.Job.Bil2IntegrationsMonitoring.Domain.Metrics;
using Lykke.Job.Bil2IntegrationsMonitoring.Domain.Services;
using Lykke.Job.Bil2IntegrationsMonitoring.DomainServices;
using Moq;
using Xunit;

namespace Lykke.Job.Bil2IntegrationsMonitoring.Tests
{
    public class TransactionExecutorMetricsCollectorServiceTests
    {
        private const string IntegrationName = "TestIntegration";

        [Fact]
        public async Task MeasureIsAliveAsync_TrExecutorIsAvailable_MetricIsCalculated()
        {
            //ARRANGE
            var metricPublisher = new Mock<IMetricPublisher>();
            var transactionExecutorApi = new Mock<ITransactionsExecutorApi>();
            var transactionExecutorMetricsCollectorService =
                new TransactionExecutorMetricsCollectorService(
                    IntegrationName,
                    metricPublisher.Object,
                    transactionExecutorApi.Object);
            var delay = TimeSpan.FromMilliseconds(200);

            metricPublisher
                .Setup(x => x.PublishGaugeAsync(It.IsNotNull<IMetric>(), It.IsAny<KeyValuePair<string, string>[]>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            transactionExecutorApi
                .Setup(x => x.GetIsAliveAsync())
                .Returns(() =>
                {
                    Task.Delay(delay).Wait();

                    return Task.FromResult(new TransactionsExecutorIsAliveResponse(IntegrationName,
                        new Version(1, 0, 0, 0),
                        "test env",
                        new Version(1, 0, 0, 0)));
                })
                .Verifiable();

            //ACT
            await transactionExecutorMetricsCollectorService.MeasureIsAliveAsync();

            //ASSERT

            transactionExecutorApi.Verify(x => x.GetIsAliveAsync(),
                Times.Once);
            metricPublisher
                .Verify(x =>
                        x.PublishGaugeAsync(It.Is<TransactionExecutorIsAliveResponseTimeMetric>(y => IsCorrectlyCalculated(y, delay)),
                            It.IsAny<KeyValuePair<string, string>[]>()),
                    Times.Once);

            metricPublisher
                .Verify(x =>
                        x.PublishGaugeAsync(It.Is<TransactionExecutorDiseasePresenseMetric>(y => y.Value == 0),
                            It.IsAny<KeyValuePair<string, string>[]>()),
                    Times.Once);
        }

        [Fact]
        public async Task MeasureIsAliveAsync_TrExecutorIsNotAvailable_MetricIsCalculated()
        {
            //ARRANGE
            var metricPublisher = new Mock<IMetricPublisher>();
            var transactionExecutorApi = new Mock<ITransactionsExecutorApi>();
            var transactionExecutorMetricsCollectorService =
                new TransactionExecutorMetricsCollectorService(
                    IntegrationName,
                    metricPublisher.Object,
                    transactionExecutorApi.Object);
            var delay = TimeSpan.FromMilliseconds(200);

            metricPublisher
                .Setup(x => x.PublishGaugeAsync(It.IsNotNull<IMetric>(), It.IsAny<KeyValuePair<string, string>[]>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            transactionExecutorApi
                .Setup(x => x.GetIsAliveAsync())
                .ThrowsAsync(new Exception("Error"), delay)
                .Verifiable();

            //ACT
            await transactionExecutorMetricsCollectorService.MeasureIsAliveAsync();

            //ASSERT

            transactionExecutorApi.Verify(x => x.GetIsAliveAsync(),
                Times.Once);
            metricPublisher
                .Verify(x =>
                        x.PublishGaugeAsync(It.Is<TransactionExecutorIsAliveResponseTimeMetric>(y => IsCorrectlyCalculated(y, delay)),
                            It.IsAny<KeyValuePair<string, string>[]>()),
                    Times.Once);

            metricPublisher
                .Verify(x =>
                        x.PublishGaugeAsync(It.Is<TransactionExecutorDiseasePresenseMetric>(y => y.Value == 1),
                            It.IsAny<KeyValuePair<string, string>[]>()),
                    Times.Once);
        }

        [Fact]
        public async Task MeasureDependencyVersionsAsync_TrExecutorIsAvailable_MetricIsCalculated()
        {
            //ARRANGE
            var metricPublisher = new Mock<IMetricPublisher>();
            var transactionExecutorApi = new Mock<ITransactionsExecutorApi>();
            var transactionExecutorMetricsCollectorService =
                new TransactionExecutorMetricsCollectorService(
                    IntegrationName,
                    metricPublisher.Object,
                    transactionExecutorApi.Object);
            var depName0 = "node-dep-0";
            var depName1 = "node-dep-1";

            metricPublisher
                .Setup(x => x.PublishGaugeAsync(It.IsNotNull<IMetric>(), It.IsAny<KeyValuePair<string, string>[]>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            transactionExecutorApi
                .Setup(x => x.GetIntegrationInfoAsync())
                .Returns(() =>
                {

                    var blockchainInfo = new BlockchainInfo(1, DateTime.UtcNow - TimeSpan.FromMinutes(1));
                    var dependencyInfo = new Dictionary<string, DependencyInfo>()
                    {
                        {
                            depName0,
                            new DependencyInfo(
                                new Version(1,0,0),
                                new Version(1,0,0))
                        },
                        {
                            depName1,
                            new DependencyInfo(
                                new Version(1,0,0),
                                new Version(1,1,0))
                        }
                    };

                    return Task.FromResult(new IntegrationInfoResponse(blockchainInfo, dependencyInfo));
                })
                .Verifiable();

            //ACT
            await transactionExecutorMetricsCollectorService.MeasureDependencyVersionsAsync();

            //ASSERT

            transactionExecutorApi.Verify(x => x.GetIntegrationInfoAsync(),
                Times.Once);

            metricPublisher
                .Verify(x =>
                        x.PublishGaugeAsync(It.Is<NewDependencyVersionAvailableMetric>(
                                y => y.Value == 1 && y.Name == string.Format(NewDependencyVersionAvailableMetric.MetricName, depName1)),
                            It.IsAny<KeyValuePair<string, string>[]>()),
                    Times.Once);

            metricPublisher
                .Verify(x =>
                        x.PublishGaugeAsync(It.Is<NewDependencyVersionAvailableMetric>(
                                y => y.Value == 0 && y.Name == string.Format(NewDependencyVersionAvailableMetric.MetricName, depName0)),
                            It.IsAny<KeyValuePair<string, string>[]>()),
                    Times.Once);
        }

        [Fact]
        public async Task MeasureGetInfoAsync_TrExecutorIsAvailable_MetricIsCalculated()
        {
            //ARRANGE
            var metricPublisher = new Mock<IMetricPublisher>();
            var transactionExecutorApi = new Mock<ITransactionsExecutorApi>();
            var transactionExecutorMetricsCollectorService =
                new TransactionExecutorMetricsCollectorService(
                    IntegrationName,
                    metricPublisher.Object,
                    transactionExecutorApi.Object);
            var delay = TimeSpan.FromMilliseconds(200);
            var depName0 = "node-dep-0";
            var depName1 = "node-dep-1";

            metricPublisher
                .Setup(x => x.PublishGaugeAsync(It.IsNotNull<IMetric>(), It.IsAny<KeyValuePair<string, string>[]>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            transactionExecutorApi
                .Setup(x => x.GetIntegrationInfoAsync())
                .Returns(() =>
                {
                    Task.Delay(delay).Wait();

                    var blockchainInfo = new BlockchainInfo(1, DateTime.UtcNow - TimeSpan.FromMinutes(1));
                    var dependencyInfo = new Dictionary<string, DependencyInfo>()
                    {
                    };

                    return Task.FromResult(new IntegrationInfoResponse(blockchainInfo, dependencyInfo));
                })
                .Verifiable();

            //ACT
            await transactionExecutorMetricsCollectorService.MeasureGetInfoAsync();

            //ASSERT
            transactionExecutorApi.Verify(x => x.GetIntegrationInfoAsync(),
                Times.Once);

            metricPublisher
                .Verify(x =>
                        x.PublishGaugeAsync(It.Is<TransactionExecutorIntegrationInfoResponseTimeMetric>(
                                y => IsCorrectlyCalculated(y,delay)),
                            It.IsAny<KeyValuePair<string, string>[]>()),
                    Times.Once);

            metricPublisher
                .Verify(x =>
                        x.PublishGaugeAsync(It.Is<TimeFromLastBlockUpdateMetric>(
                                y => y.Value >= 60 && y.Value <= 70),
                            It.IsAny<KeyValuePair<string, string>[]>()),
                    Times.Once);
        }

        private static bool IsCorrectlyCalculated(IMetric metric, TimeSpan delay)
        {
            var approximation = metric.Value / (delay.TotalMilliseconds / 1_000d);
            //max 15 % diff 

            return approximation <= 1.15 && approximation >= 1.0;
        }
    }
}
