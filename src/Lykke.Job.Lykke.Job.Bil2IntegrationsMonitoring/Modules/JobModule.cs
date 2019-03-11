using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common;
using Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.Domain.Services;
using Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.PeriodicalHandlers;
using Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.RabbitPublishers;
using Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.RabbitSubscribers;
using Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.Services;
using Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.Settings.JobSettings;
using Lykke.JobTriggers.Extenstions;
using Lykke.JobTriggers.Triggers;
using Lykke.Sdk;
using Lykke.Sdk.Health;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.Modules
{
    public class JobModule : Module
    {
        private readonly MonitoringJobSettings _settings;
        private readonly IReloadingManager<MonitoringJobSettings> _settingsManager;
        // NOTE: you can remove it if you don't need to use IServiceCollection extensions to register service specific dependencies
        private readonly IServiceCollection _services;

        public JobModule(MonitoringJobSettings settings, IReloadingManager<MonitoringJobSettings> settingsManager)
        {
            _settings = settings;
            _settingsManager = settingsManager;

            _services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            // NOTE: Do not register entire settings in container, pass necessary settings to services which requires them
            // ex:
            // builder.RegisterType<QuotesPublisher>()
            //  .As<IQuotesPublisher>()
            //  .WithParameter(TypedParameter.From(_settings.Rabbit.ConnectionString))

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

            RegisterAzureQueueHandlers(builder);

            RegisterPeriodicalHandlers(builder);

            RegisterRabbitMqSubscribers(builder);

            RegisterRabbitMqPublishers(builder);

            // TODO: Add your dependencies here

            builder.Populate(_services);
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
                    pool.AddDefaultConnection(_settingsManager.Nested(s => s.AzureQueue.ConnectionString));
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

            builder.RegisterType<MyRabbitSubscriber>()
                .As<IStartable>()
                .SingleInstance()
                .WithParameter("connectionString", _settings.Rabbit.ConnectionString)
                .WithParameter("exchangeName", _settings.Rabbit.ExchangeName);
        }

        private void RegisterRabbitMqPublishers(ContainerBuilder builder)
        {
            // TODO: You should register each publisher in DI container as publisher specific interface and as IStartable,
            // as singleton and do not autoactivate it

            builder.RegisterType<MyRabbitPublisher>()
                .As<IMyRabbitPublisher>()
                .As<IStartable>()
                .SingleInstance()
                .WithParameter(TypedParameter.From(_settings.Rabbit.ConnectionString));
        }
    }
}
