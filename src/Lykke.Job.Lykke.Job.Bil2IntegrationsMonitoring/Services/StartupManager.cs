using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.JobTriggers.Triggers;
using Lykke.Sdk;

namespace Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.Services
{
    // NOTE: Sometimes, startup process which is expressed explicitly is not just better, 
    // but the only way. If this is your case, use this class to manage startup.
    // For example, sometimes some state should be restored before any periodical handler will be started, 
    // or any incoming message will be processed and so on.
    // Do not forget to remove As<IStartable>() and AutoActivate() from DI registartions of services, 
    // which you want to startup explicitly.

    public class StartupManager : IStartupManager
    {
        private readonly ILog _log;
        private readonly TriggerHost _triggerHost;

        public StartupManager(
            ILogFactory logFactory,
            TriggerHost triggerHost)
        {
            _log = logFactory.CreateLog(this);
            _triggerHost = triggerHost;
        }

        public async Task StartAsync()
        {
            await _triggerHost.Start();
        }
    }
}
