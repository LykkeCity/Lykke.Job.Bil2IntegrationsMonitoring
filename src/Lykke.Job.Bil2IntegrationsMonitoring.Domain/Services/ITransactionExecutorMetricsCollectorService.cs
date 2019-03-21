using System.Threading.Tasks;

namespace Lykke.Job.Bil2IntegrationsMonitoring.Domain.Services
{
    public interface ITransactionExecutorMetricsCollectorService
    {
        Task MeasureIsAliveAsync();

        Task MeasureGetInfoAsync();

        Task MeasureDependencyVersionsAsync();
    }
}
