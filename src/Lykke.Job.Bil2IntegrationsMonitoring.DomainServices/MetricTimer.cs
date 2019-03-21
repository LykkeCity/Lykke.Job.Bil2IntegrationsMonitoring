using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Lykke.Job.Bil2IntegrationsMonitoring.Domain.Metrics;

namespace Lykke.Job.Bil2IntegrationsMonitoring.DomainServices
{
    public class MetricTimer : IDisposable
    {
        private Stopwatch _sw;
        private MetricBase _metric;

        public MetricTimer(MetricBase metric)
        {
            _metric = metric;
            _sw = new Stopwatch();
            _sw.Start();
        }

        public void Dispose()
        {
            _metric.Set(CalculateSeconds(_sw.Elapsed));
        }

        public static async Task MeasureSafelyAsync(
            Func<Task> func, 
            MetricBase metric)
        {
            try
            {
                using (MetricTimer timer = new MetricTimer(metric))
                {
                    await func();
                }
            }
            catch (TimeoutException e)
            {
            }
            catch (Exception e)
            {
            }
        }

        private static double CalculateSeconds(TimeSpan timeSpan)
        {
            return timeSpan.TotalMilliseconds / 1_000d;
        }
    }
}
