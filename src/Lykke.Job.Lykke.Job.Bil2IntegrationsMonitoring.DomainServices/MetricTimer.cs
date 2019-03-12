using Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.Domain.Metrics;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.DomainServices
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

        public static async Task MeasureSafelyWithTimeoutAsync(
            Func<Task> func, 
            MetricBase metric, 
            TimeSpan timeout)
        {
            try
            {
                var measureTask = func();
                var timeoutTask = Task.Delay(timeout);
                Task completedTask;

                using (MetricTimer timer = new MetricTimer(metric))
                {
                    completedTask = await Task.WhenAny(measureTask, timeoutTask);
                }

                if (completedTask == timeoutTask)
                    metric.Set(CalculateSeconds(timeout));
            }
            catch (Exception e)
            {
                metric.Set(CalculateSeconds(timeout));
            }
        }

        private static double CalculateSeconds(TimeSpan timeSpan)
        {
            return timeSpan.Milliseconds / 1_000d;
        }
    }
}
