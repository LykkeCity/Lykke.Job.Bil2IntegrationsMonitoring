using Autofac;
using Common;
using Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.Domain.Metrics;
using Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.Domain.Services;
using Prometheus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.DomainServices
{
    /*
        Metrics which should be tracked to the Push gateway:
        Response time of the Sign service isAlive endpoint.
        Response time of the Transactions executor isAlive endpoint.
        Response time of the Blocks reader isAlive endpoint.
        Response time of the Transactions executor Integration info endpoint.
        Presence of the Integration disease. Could be obtained via isAlive of transactions executor
        Time elapsed after block number update. 
        Number moment of the latest block could be obtained via Integration info endpoint.
        Availability of an update of a dependency. List of the dependencies with their 
        running and available versions could be obtained via Integration info endpoint.
    */
    public class MetricPublisher : IMetricPublisher, IStartable, IStopable
    {
        private readonly MetricPusher _metricPusher;

        public MetricPublisher(MetricPusher metricPusher)
        {
            _metricPusher = metricPusher;
        }

        public Task PublishGaugeAsync(
            IMetric metric,
            params KeyValuePair<string, string>[] additionalLabels)
        {
            var integrationName = metric.IntegrationName;
            var gauge = Metrics.CreateGauge(
                name: $"bil2_{integrationName}_{metric.Name}",
                help: $"measuring {metric.Name}",
                configuration: new GaugeConfiguration
                {
                    SuppressInitialValue = false,
                    LabelNames = additionalLabels.Select(p => p.Key).ToArray()
                });

            gauge.WithLabels(additionalLabels.Select(p => p.Value).ToArray())
                .Set(metric.Value);

            return Task.CompletedTask;
        }

        public void Start()
        {
            _metricPusher.Start();
        }

        public void Dispose()
        {
            (_metricPusher as IDisposable)?.Dispose();
        }

        public void Stop()
        {
            _metricPusher.Stop();
        }
    }
}
