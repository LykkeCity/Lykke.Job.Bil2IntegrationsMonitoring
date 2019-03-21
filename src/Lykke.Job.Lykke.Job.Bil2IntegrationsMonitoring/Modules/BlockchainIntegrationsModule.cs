using Autofac;
using Autofac.Extensions.DependencyInjection;
using Lykke.Bil2.Client.SignService.Services;
using Lykke.Bil2.Client.TransactionsExecutor.Services;
using Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.Domain.Services;
using Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.Services.Factories;
using Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.Settings;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.Modules
{
    public class BlockchainIntegrationsModule : Module
    {
        private readonly IReloadingManager<AppSettings> _settings;
        private readonly IServiceCollection _services;

        public BlockchainIntegrationsModule(IReloadingManager<AppSettings> settings)
        {
            _settings = settings;

            _services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            _services.AddSingleton<ITransactionExecutorMetricsCollectorFactory>(ctx =>
            {
                return new TransactionExecutorMetricsCollectorFactory(
                    ctx.GetRequiredService<IMetricPublisher>(),
                    ctx.GetRequiredService<ITransactionsExecutorApiProvider>());
            });

            _services.AddSingleton<ISignServiceMetricsCollectorServiceFactory>(ctx =>
            {
                return new SignServiceMetricsCollectorServiceFactory(
                    ctx.GetRequiredService<IMetricPublisher>(),
                    ctx.GetRequiredService<ISignServiceApiProvider>());
            });

            builder.Populate(_services);
        }
    }
}
