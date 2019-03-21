using Autofac;
using Autofac.Extensions.DependencyInjection;
using Lykke.Bil2.Client.SignService;
using Lykke.Bil2.Client.SignService.Services;
using Lykke.Bil2.Client.TransactionsExecutor;
using Lykke.Bil2.Client.TransactionsExecutor.Services;
using Lykke.Job.Bil2IntegrationsMonitoring.Domain.Services;
using Lykke.Job.Bil2IntegrationsMonitoring.Services;
using Lykke.Job.Bil2IntegrationsMonitoring.Services.Factories;
using Lykke.Job.Bil2IntegrationsMonitoring.Settings;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Job.Bil2IntegrationsMonitoring.Modules
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

            _services.AddSignServiceClient((options) =>
            {
                options.Timeout = _settings.CurrentValue.Bil2MonitoringJobSettings.BlockchainIntegrationTimeout;
                foreach (var integration in _settings.CurrentValue.BlockchainIntegrations.Integrations)
                {
                    options.AddIntegration(integration.Name, (signServiceOptions) =>
                    {
                        signServiceOptions.Url = integration.SignServiceUrl;
                    });
                }
            });

            _services.AddTransactionsExecutorClient((options) =>
            {
                options.Timeout = _settings.CurrentValue.Bil2MonitoringJobSettings.BlockchainIntegrationTimeout;
                foreach (var integration in _settings.CurrentValue.BlockchainIntegrations.Integrations)
                {
                    options.AddIntegration(integration.Name, (transactionExecutorOptions) =>
                    {
                        transactionExecutorOptions.Url = integration.TransactionExecutorUrl;
                    });
                }
            });

            _services.AddSingleton(
                new BlockchainIntegrationResolver(_settings.CurrentValue.BlockchainIntegrations.Integrations));

            builder.Populate(_services);
        }
    }
}
