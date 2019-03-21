using System.Threading.Tasks;

namespace Lykke.Job.Bil2IntegrationsMonitoring.Services.Factories
{
    public interface ISignServiceMetricsCollectorServiceFactory
    {
        Task MeasureIsAliveAsync(string integrationName);
    }
}
