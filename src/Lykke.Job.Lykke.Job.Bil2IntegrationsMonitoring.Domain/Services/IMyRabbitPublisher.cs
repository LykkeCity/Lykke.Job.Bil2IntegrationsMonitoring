using System.Threading.Tasks;
using Autofac;
using Common;
using Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.Contract;

namespace Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.Domain.Services
{
    public interface IMyRabbitPublisher : IStartable, IStopable
    {
        Task PublishAsync(MyPublishedMessage message);
    }
}