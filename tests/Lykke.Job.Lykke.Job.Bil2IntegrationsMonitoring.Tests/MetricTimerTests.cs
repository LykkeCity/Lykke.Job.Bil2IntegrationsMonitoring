using System;
using System.Threading;
using System.Threading.Tasks;
using Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.DomainServices;
using Xunit;

namespace Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.Tests
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

            Assert.True(metric.Value >= dealyMs / 1_000d);
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

            Assert.True(metric.Value >= dealyMs / 1_000d);
        }

        [Fact]
        public async Task MeasureSafelyWithTimeoutAsync_FastExecution_CalculatesTime()
        {
            var dealyMs = 100;
            var metric = new TestMetric(_integrationName);

            await MetricTimer.MeasureSafelyWithTimeoutAsync(async () =>
                {
                    await Task.Delay(dealyMs);
                },
                metric,
                TimeSpan.FromMinutes(1));

            Assert.True(metric.Value >= dealyMs / 1_000d);
        }

        [Fact]
        public async Task MeasureSafelyWithTimeoutAsync_SlowExecution_MetricsEqualToTimeout()
        {
            var dealyMs = 10000;
            var timeoutMs = 200;
            var metric = new TestMetric(_integrationName);

            await MetricTimer.MeasureSafelyWithTimeoutAsync(async () =>
                {
                    await Task.Delay(dealyMs);
                },
                metric,
                TimeSpan.FromMilliseconds(timeoutMs));

            Assert.True(metric.Value >= timeoutMs / 1_000d);
        }
    }
}
