using Lykke.Bil2.Client.SignService;
using Lykke.Bil2.Contract.Common.Responses;
using Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.Domain.Metrics;
using Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.Domain.Services;
using Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.DomainServices;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Bil2.Client.SignService.Services;
using Xunit;

namespace Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.Tests
{
    public class SignServiceMetricsCollectorServiceTests
    {
        //https://stackoverflow.com/questions/155436/unit-test-naming-best-practices
        //UnitOfWork_StateUnderTest_ExpectedBehavior]
        [Fact]
        public async Task MeasureIsAliveAsync_SignServiceIsAvailable_MetricIsCalculated()
        {
            //ARRANGE
            Mock<ISignServiceApi> signServiceApi = new Mock<ISignServiceApi>();
            Mock<ISignServiceApiProvider> signServiceApiProvider = new Mock<ISignServiceApiProvider>();
            Mock<IMetricPublisher> metricPublisher = new Mock<IMetricPublisher>();

            signServiceApi.Setup(x => x.GetIsAliveAsync())
                .Returns(() =>
                {
                    var result = new BlockchainIsAliveResponse("TestName",
                        new Version(1, 0, 1, 0),
                        "testEnv",
                        new Version(1, 0, 0, 0));
                    Task.Delay(333).Wait();

                    return Task.FromResult(result);
                })
                .Verifiable();

            signServiceApiProvider.Setup(x => x.GetApi(It.IsAny<string>()))
                .Returns(signServiceApi.Object);

            metricPublisher
                .Setup(x => x.PublishGaugeAsync(It.IsNotNull<IMetric>(), It.IsAny<KeyValuePair<string, string>[]>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            SignServiceMetricsCollectorService signServiceCollector =
                new SignServiceMetricsCollectorService("TestIntegration",
                    metricPublisher.Object,
                    signServiceApiProvider.Object);

            //ACT
            await signServiceCollector.MeasureIsAliveAsync();

            //ASSERT
            signServiceApi.Verify(x => x.GetIsAliveAsync(), Times.Once);
            metricPublisher
                .Verify(x => 
                    x.PublishGaugeAsync(It.Is<IMetric>(y => y.Value >= 0.333), 
                        It.IsAny<KeyValuePair<string, string>[]>()), 
                    Times.Once);
        }

        [Fact]
        public async Task MeasureIsAliveAsync_SignServiceIsUnavailable_MetricIsNotCalculated()
        {
            //ARRANGE
            var timeout = TimeSpan.FromMilliseconds(500);
            Mock<ISignServiceApi> signServiceApi = new Mock<ISignServiceApi>();
            Mock<IMetricPublisher> metricPublisher = new Mock<IMetricPublisher>();
            Mock<ISignServiceApiProvider> signServiceApiProvider = new Mock<ISignServiceApiProvider>();

            signServiceApi.Setup(x => x.GetIsAliveAsync())
                .ThrowsAsync(new TimeoutException("Something went wrong."), timeout)
                .Verifiable();

            signServiceApiProvider.Setup(x => x.GetApi(It.IsAny<string>()))
                .Returns(signServiceApi.Object);

            metricPublisher
                .Setup(x => x.PublishGaugeAsync(It.IsNotNull<IMetric>(), It.IsAny<KeyValuePair<string, string>[]>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            SignServiceMetricsCollectorService signServiceCollector =
                new SignServiceMetricsCollectorService("TestIntegration",
                    metricPublisher.Object,
                    signServiceApiProvider.Object);

            //ACT
            await signServiceCollector.MeasureIsAliveAsync();

            //ASSERT
            signServiceApi.Verify(x => x.GetIsAliveAsync(), Times.Once);
            metricPublisher
                .Verify(x =>
                        x.PublishGaugeAsync(It.Is<IMetric>(y => y.Value >= timeout.Seconds),
                            It.IsAny<KeyValuePair<string, string>[]>()),
                    Times.Once);
        }
    }
}
