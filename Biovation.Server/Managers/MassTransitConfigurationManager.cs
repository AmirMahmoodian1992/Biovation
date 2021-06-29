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

            var rabbitConfigurationSection = configuration.GetSection("MassTransit").GetChildren().FirstOrDefault(c => string.Equals(c.Key, "RabbitMQ", StringComparison.InvariantCultureIgnoreCase))?.GetChildren();
            var receiveEndpoints = rabbitConfigurationSection?.FirstOrDefault(c =>
                string.Equals(c.Key, "ReceiveEndpoints", StringComparison.InvariantCultureIgnoreCase))?.GetChildren().ToList();

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
                        var receiveEndpointConfig = receiveEndpoints?.FirstOrDefault(c =>
                            c.GetChildren()?.FirstOrDefault(cc => string.Equals(cc.Key, "Consumer", StringComparison.InvariantCultureIgnoreCase))
                                ?.GetChildren()?.FirstOrDefault(ccc =>
                                    string.Equals(ccc.Key, "name", StringComparison.InvariantCultureIgnoreCase))?.Value == consumer.Name);

                        var consumerConfig = receiveEndpointConfig?.GetChildren().FirstOrDefault(ec =>
                            string.Equals(ec.Key, "Consumer", StringComparison.InvariantCultureIgnoreCase));

                        cfg.ReceiveEndpoint(consumerConfig?["queueName"] ?? consumer.Name, e =>
                       {
                           e.Bind(consumerConfig?["exchangeName"] ?? consumer.Name);
                           e.ConfigureConsumer(context, consumer);
                       });
                    }
                });
            });

            services.AddMassTransitHostedService();
        }
    }
}
