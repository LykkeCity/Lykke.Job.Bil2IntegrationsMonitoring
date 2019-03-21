using System.Threading.Tasks;

namespace Lykke.Job.Bil2IntegrationsMonitoring.Services.Factories
{
    public interface ITransactionExecutorMetricsCollectorFactory
    {
        Task MeasureIsAliveAsync(string integrationName);

        Task MeasureDependencyVersionsAsync(string integrationName);

        Task MeasureGetInfoAsync(string integrationName);
    }
}
