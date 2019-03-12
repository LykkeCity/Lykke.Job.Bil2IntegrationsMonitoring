using System.Threading.Tasks;

namespace Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.Domain.Services
{
    public interface ISignServiceMetricsCollectorService
    {
        Task MeasureIsAliveAsync();
    }
}
