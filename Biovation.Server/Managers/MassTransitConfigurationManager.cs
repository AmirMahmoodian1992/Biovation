using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;

namespace Biovation.Server.Managers
{
    public static class MassTransitConfigurationManager
    {
        public static void ConfigureMassTransit(this IServiceCollection services)
        {
            services.Configure<HealthCheckPublisherOptions>(options =>
            {
                options.Delay = TimeSpan.FromSeconds(2);
                options.Predicate = check => check.Tags.Contains("ready");
            });

            services.AddMassTransit(config =>
            {
                config.UsingRabbitMq();
            });

            services.AddMassTransitHostedService();
        }
    }
}
