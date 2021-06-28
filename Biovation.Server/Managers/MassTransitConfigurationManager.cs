using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Linq;

namespace Biovation.Server.Managers
{
    public static class MassTransitConfigurationManager
    {
        public static void ConfigureMassTransit(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<HealthCheckPublisherOptions>(options =>
            {
                options.Delay = TimeSpan.FromSeconds(2);
                options.Predicate = check => check.Tags.Contains("ready");
            });

            var consumers = typeof(MassTransitConfigurationManager).Assembly.GetTypes()
                .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition)
                .Where(t => typeof(IConsumer).IsAssignableFrom(t) || t.Name.EndsWith("Consumer", StringComparison.InvariantCultureIgnoreCase))
                .ToList();


            services.AddMassTransit(config =>
            {
                config.AddConsumers(consumers.ToArray());
                config.UsingRabbitMq((context, cfg) =>
                {
                    foreach (var consumer in consumers)
                    {
                        cfg.ReceiveEndpoint(configuration.GetValue<string>("MassTransit:RabbitMQ:ReceiveEndpoints:Consumer:queueName"), e =>
                       {
                           e.Bind(configuration.GetValue<string>("MassTransit:RabbitMQ:ReceiveEndpoints:Consumer:exchangeName"));
                           e.ConfigureConsumer(context, consumer);
                       });
                    }
                });
            });

            services.AddMassTransitHostedService();
        }
    }
}
