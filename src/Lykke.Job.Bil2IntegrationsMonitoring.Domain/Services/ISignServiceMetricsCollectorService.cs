using System.Threading.Tasks;

namespace Lykke.Job.Bil2IntegrationsMonitoring.Domain.Services
{
    public interface ISignServiceMetricsCollectorService
    {
        Task MeasureIsAliveAsync();
    }
}
