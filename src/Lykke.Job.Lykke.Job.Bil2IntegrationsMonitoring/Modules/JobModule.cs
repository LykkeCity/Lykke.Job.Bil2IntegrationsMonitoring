﻿using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common;
using Hangfire;
using Hangfire.MemoryStorage;
using Lykke.Bil2.Client.SignService;
using Lykke.Bil2.Client.TransactionsExecutor;
using Lykke.Common.Log;
using Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.Domain.Services;
using Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.DomainServices;
using Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.PeriodicalHandlers;
using Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.Services;
using Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.Settings;
using Lykke.JobTriggers.Extenstions;
using Lykke.JobTriggers.Triggers;
using Lykke.Logs.Hangfire;
using Lykke.Sdk;
using Lykke.Sdk.Health;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.Modules
{
    public class JobModule : Module
    {
        private readonly IReloadingManager<AppSettings> _settings;
        private readonly IServiceCollection _services;

        public JobModule(IReloadingManager<AppSettings> settings)
        {
            _settings = settings;

            _services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(ctx =>
                {
                    var scope = ctx.Resolve<ILifetimeScope>();
                    var host = new TriggerHost(new AutofacServiceProvider(scope));
                    return host;
                }).As<TriggerHost>()
                .SingleInstance();

            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>()
                .SingleInstance();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>()
                .AutoActivate()
                .SingleInstance();

            builder
                .RegisterBuildCallback(StartHangfireServer)
                .Register(ctx => new BackgroundJobServer())
                .SingleInstance();

            //Add Prometheus
            builder.RegisterType<MetricPublisher>()
                .WithParameter(TypedParameter.From(new Prometheus.MetricPusher(
                    _settings.CurrentValue
                        .Bil2MonitoringJobSettings.PrometheusPushGatewayUrl.RemoveLastSymbolIfExists('/') + "/metrics",
                    "bil-monitoring-2-job")))
                .As<IMetricPublisher>()
                .As<IStartable>()
                .As<IStopable>()
                .SingleInstance();

            _services.AddSignServiceClient((options) =>
            {
                options.Timeout = _settings.CurrentValue.Bil2MonitoringJobSettings.BlockchainIntegrationTimeout;
                foreach (var integration in _settings.CurrentValue.Bil2MonitoringJobSettings.BlockchainIntegrations.Integrations)
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
                foreach (var integration in _settings.CurrentValue.Bil2MonitoringJobSettings.BlockchainIntegrations.Integrations)
                {
                    options.AddIntegration(integration.Name, (transactionExecutorOptions) =>
                    {
                        transactionExecutorOptions.Url = integration.TransactionExecutorUrl;
                    });
                }
            });

            _services.AddSingleton(
                new BlockchainIntegrationResolver(_settings.CurrentValue.Bil2MonitoringJobSettings
                    .BlockchainIntegrations.Integrations));

            builder.Populate(_services);
        }

        private void StartHangfireServer(IContainer container)
        {
            var logProvider = new LykkeLogProvider(container.Resolve<ILogFactory>());
            GlobalConfiguration.Configuration.UseMemoryStorage();
            GlobalConfiguration.Configuration.UseLogProvider(logProvider)
                .UseAutofacActivator(container);

            container.Resolve<BackgroundJobServer>();
        }

        private void RegisterAzureQueueHandlers(ContainerBuilder builder)
        {
            builder.Register(ctx =>
            {
                var scope = ctx.Resolve<ILifetimeScope>();
                var host = new TriggerHost(new AutofacServiceProvider(scope));
                return host;
            }).As<TriggerHost>()
            .SingleInstance();
    
            // NOTE: You can implement your own poison queue notifier for azure queue subscription.
            // See https://github.com/LykkeCity/JobTriggers/blob/master/readme.md
            // builder.Register<PoisionQueueNotifierImplementation>().As<IPoisionQueueNotifier>();

            builder.AddTriggers(
                pool =>
                {
                });
        }

        private void RegisterPeriodicalHandlers(ContainerBuilder builder)
        {
            // TODO: You should register each periodical handler in DI container as IStartable singleton and autoactivate it

            builder.RegisterType<MyPeriodicalHandler>()
                .As<IStartable>()
                .As<IStopable>()
                .SingleInstance();
        }

        private void RegisterRabbitMqSubscribers(ContainerBuilder builder)
        {
            // TODO: You should register each subscriber in DI container as IStartable singleton and autoactivate it

            //builder.RegisterType<MyRabbitSubscriber>()
            //    .As<IStartable>()
            //    .SingleInstance()
            //    .WithParameter("connectionString", _settings.Rabbit.ConnectionString)
            //    .WithParameter("exchangeName", _settings.Rabbit.ExchangeName);
        }

        private void RegisterRabbitMqPublishers(ContainerBuilder builder)
        {
            // TODO: You should register each publisher in DI container as publisher specific interface and as IStartable,
            // as singleton and do not autoactivate it

            //builder.RegisterType<MyRabbitPublisher>()
            //    .As<IMyRabbitPublisher>()
            //    .As<IStartable>()
            //    .SingleInstance()
            //    .WithParameter(TypedParameter.From(_settings.Rabbit.ConnectionString));
        }
    }
}
