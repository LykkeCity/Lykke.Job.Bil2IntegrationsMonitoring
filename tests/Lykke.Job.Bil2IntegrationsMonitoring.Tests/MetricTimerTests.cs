using System;
using System.Threading.Tasks;
using Lykke.Job.Bil2IntegrationsMonitoring.DomainServices;
using Xunit;

namespace Lykke.Job.Bil2IntegrationsMonitoring.Tests
{
    public class MetricTimerTests
    {
        private static string _integrationName = "TestIntegration";

        [Fact]
        public async Task Dispose_MetricTimer_CalculatesTime()
        {
            var dealyMs = 100;
            var metric = new TestMetric(_integrationName);
            MetricTimer timer = new MetricTimer(metric);

            await Task.Delay(dealyMs);
            timer.Dispose();

            AssertMetric(metric, dealyMs);
        }

        [Fact]
        public async Task DisposeWithUsing_MetricTimer_CalculatesTime()
        {
            var dealyMs = 100;
            var metric = new TestMetric(_integrationName);
            using (var timer = new MetricTimer(metric))
            {
                await Task.Delay(dealyMs);
            }

            AssertMetric(metric, dealyMs);
        }

        [Fact]
        public async Task MeasureSafelyAsync_FastExecution_CalculatesTime()
        {
            var dealyMs = 100;
            var metric = new TestMetric(_integrationName);

            await MetricTimer.MeasureSafelyAsync(async () =>
                {
                    await Task.Delay(dealyMs);
                },
                metric);

            AssertMetric(metric, dealyMs);
        }

        [Fact]
        public async Task MeasureSafelyAsync_SlowExecution_MetricsEqualToTimeout()
        {
            var timeoutMs = 200;
            var metric = new TestMetric(_integrationName);

            await MetricTimer.MeasureSafelyAsync(async () =>
                {
                    await Task.Delay(timeoutMs);
                    throw new TimeoutException();
                },
                metric);

            AssertMetric(metric, timeoutMs);
        }

        private static void AssertMetric(TestMetric metric, int timeoutMs)
        {
            var approximation = metric.Value / (timeoutMs / 1_000d);
            //max 15 % diff 
            Assert.True(approximation <= 1.15);
            Assert.True(approximation >= 1.0);
        }
    }
}
