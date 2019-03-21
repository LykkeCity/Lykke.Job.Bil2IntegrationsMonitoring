using Common.Log;
using Hangfire;
using Lykke.Common.Log;
using Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.Services.Factories;
using Lykke.JobTriggers.Triggers;
using Lykke.Sdk;
using System;
using System.Threading.Tasks;

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
        private BlockchainIntegrationResolver _resolver;

        public StartupManager(
            ILogFactory logFactory,
            TriggerHost triggerHost,
            BlockchainIntegrationResolver resolver)
        {
            _log = logFactory.CreateLog(this);
            _triggerHost = triggerHost;
            _resolver = resolver;
        }

        public async Task StartAsync()
        {
            await _triggerHost.Start();

            var allIntegrations = _resolver.GetAllIntegrationNames();

            foreach (var integration in allIntegrations)
            {
                string integrationName = integration;

                RegisterSignServiceIsAliveMetricJob(integrationName);

                RegisterTransactionExecutorIsAliveMetricJob(integrationName);

                RegisterTransactionExecutorDependencyVersionsMetricJob(integrationName);

                RegisterTransactionExecutorGetInfoJob(integrationName);
            }
        }

        private static void RegisterTransactionExecutorGetInfoJob(string integrationName)
        {
            string jobId = (integrationName + "GetInfo").ToLowerInvariant();
            RecurringJob.AddOrUpdate<ITransactionExecutorMetricsCollectorFactory>(
                jobId,
                (x) => x.MeasureGetInfoAsync(integrationName),
                Cron.MinuteInterval(1),
                TimeZoneInfo.Utc);
        }

        private static void RegisterTransactionExecutorDependencyVersionsMetricJob(string integrationName)
        {
            string jobId = (integrationName + "DependencyVersions").ToLowerInvariant();
            RecurringJob.AddOrUpdate<ITransactionExecutorMetricsCollectorFactory>(
                jobId,
                (x) => x.MeasureDependencyVersionsAsync(integrationName),
                Cron.MinuteInterval(1),
                TimeZoneInfo.Utc);
        }

        private static void RegisterTransactionExecutorIsAliveMetricJob(string integrationName)
        {
            string jobId = (integrationName + "TransactionExecutorMeasureIsAlive").ToLowerInvariant();
            RecurringJob.AddOrUpdate<ITransactionExecutorMetricsCollectorFactory>(
                jobId,
                (x) => x.MeasureIsAliveAsync(integrationName),
                Cron.MinuteInterval(1),
                TimeZoneInfo.Utc);
        }

        private static void RegisterSignServiceIsAliveMetricJob(string integrationName)
        {
            string jobId = (integrationName + "SignServiceMeasureIsAlive").ToLowerInvariant();
            RecurringJob.AddOrUpdate<ISignServiceMetricsCollectorServiceFactory>(
                jobId,
                (x) => x.MeasureIsAliveAsync(integrationName),
                Cron.MinuteInterval(1),
                TimeZoneInfo.Utc);
        }
    }
}
