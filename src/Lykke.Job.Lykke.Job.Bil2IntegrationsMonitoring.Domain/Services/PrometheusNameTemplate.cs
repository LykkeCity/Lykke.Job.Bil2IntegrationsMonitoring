using System;

namespace Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.Domain.Services
{
    [AttributeUsage(AttributeTargets.All)]
    public class PrometheusNameTemplate : Attribute
    {
        public string Name { get; }

        public PrometheusNameTemplate(string name)
        {
            Name = name;
        }
    }
}
