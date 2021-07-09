using Biovation.Domain.MassTransit;
using MassTransit;
using MassTransit.RabbitMqTransport;
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

            var massTransitConfiguration = new MassTransitConfiguration();
            configuration.GetSection("MassTransit").Bind(massTransitConfiguration);

            //var rabbitConfigurationSection = configuration.GetSection("MassTransit").GetChildren().FirstOrDefault(c => string.Equals(c.Key, "RabbitMQ", StringComparison.InvariantCultureIgnoreCase))?.GetChildren();
            //var receiveEndpoints = rabbitConfigurationSection?.FirstOrDefault(c =>
            //    string.Equals(c.Key, "ReceiveEndpoints", StringComparison.InvariantCultureIgnoreCase))?.GetChildren().ToList();

            var consumers = typeof(MassTransitConfigurationManager).Assembly.GetTypes()
                .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition)
                .Where(t => typeof(IConsumer).IsAssignableFrom(t) || t.Name.EndsWith("Consumer", StringComparison.InvariantCultureIgnoreCase))
                .ToList();

            services.AddMassTransit(config =>
            {
                config.AddConsumers(consumers.ToArray());
                config.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(new RabbitMqHostAddress(massTransitConfiguration.Host?.Host ?? "localhost", massTransitConfiguration.Host?.Port, massTransitConfiguration.Host?.VirtualHost ?? string.Empty));
                    foreach (var consumer in consumers)
                    {
                        //var receiveEndpointConfig = receiveEndpoints?.FirstOrDefault(c =>
                        //    c.GetChildren()?.FirstOrDefault(cc => string.Equals(cc.Key, "Consumer", StringComparison.InvariantCultureIgnoreCase))
                        //        ?.GetChildren()?.FirstOrDefault(ccc =>
                        //            string.Equals(ccc.Key, "name", StringComparison.InvariantCultureIgnoreCase))?.Value == consumer.Name);

                        //var consumerConfig = receiveEndpointConfig?.GetChildren().FirstOrDefault(ec =>
                        //    string.Equals(ec.Key, "Consumer", StringComparison.InvariantCultureIgnoreCase));

                        var consumerConfig = massTransitConfiguration.ReceiveEndpoints.FirstOrDefault(re =>
                                string.Equals(re.Consumer.Name, consumer.Name,
                                    StringComparison.InvariantCultureIgnoreCase))
                            ?.Consumer;

                        if (consumerConfig?.Bindings != null)
                            foreach (var endpointBinding in consumerConfig.Bindings)
                                cfg.ReceiveEndpoint(consumerConfig.QueueName, e =>
                                    {
                                        {
                                            //var bindingExchangeConfig = endpointBinding?.GetChildren().FirstOrDefault(ec =>
                                            //    string.Equals(ec.Key, "exchange", StringComparison.InvariantCultureIgnoreCase));
                                            e.Bind(endpointBinding?.Exchange?.ExchangeName ?? consumer.Name, p =>
                                            {
                                                if (endpointBinding?.RoutingKey != null)
                                                    p.RoutingKey = endpointBinding.RoutingKey;
                                                if (consumerConfig.AutoDelete != null)
                                                    p.AutoDelete = (bool)consumerConfig.AutoDelete;
                                                if (endpointBinding?.Exchange?.ExchangeType != null)
                                                    p.ExchangeType = endpointBinding.Exchange?.ExchangeType;
                                                if (endpointBinding?.Exchange?.Durable != null)
                                                    p.Durable = (bool)endpointBinding.Exchange?.Durable;
                                                if (endpointBinding?.Arguments != null)
                                                    foreach (var (argumentKey, argumentValue) in endpointBinding.Arguments)
                                                    {
                                                        p.SetBindingArgument(argumentKey, argumentValue);
                                                    }

                                                if (endpointBinding?.Exchange?.Arguments != null)
                                                    foreach (var (argumentKey, argumentValue) in endpointBinding.Exchange.Arguments)
                                                    {
                                                        p.SetExchangeArgument(argumentKey, argumentValue);
                                                    }
                                            });
 
                                            if (consumerConfig.PrefetchCount != null)
                                                e.PrefetchCount = (int)consumerConfig.PrefetchCount;
                                            if (consumerConfig.PurgeOnStartup != null)
                                                e.PurgeOnStartup = (bool)consumerConfig.PurgeOnStartup;
                                        }

                                        e.ConfigureConsumer(context, consumer);
                                    });
                        else
                            cfg.ReceiveEndpoint(consumer.Name, e =>
                            {
                                e.ConfigureConsumer(context, consumer);
                            });
                    }
                });
            });

            services.AddMassTransitHostedService();
        }
    }
}
