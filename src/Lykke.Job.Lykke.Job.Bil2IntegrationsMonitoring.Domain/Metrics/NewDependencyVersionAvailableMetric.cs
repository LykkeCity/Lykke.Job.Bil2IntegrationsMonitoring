﻿using Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.Domain.Extensions;

namespace Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.Domain.Metrics
{
    public class NewDependencyVersionAvailableMetric : MetricBase
    {
        protected static readonly string MetricName = "{0}_new_dependency_version";

        public NewDependencyVersionAvailableMetric(string integrationName, string dependencyName) : base(integrationName)
        {
            Name = string.Format(MetricName, dependencyName.UseLowercaseAndUnderscoreDelimeter());
        }
    }
}
