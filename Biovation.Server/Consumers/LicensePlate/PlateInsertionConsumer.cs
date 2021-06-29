using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Biovation.Server.Consumers.LicensePlate
{
    public class PlateInsertionConsumer : IConsumer<Domain.LicensePlate>
    {
        private readonly ILogger<PlateInsertionConsumer> _logger;

        public PlateInsertionConsumer(ILogger<PlateInsertionConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<Domain.LicensePlate> context)
        {
            _logger.LogInformation(context.Message.LicensePlateNumber);
            return Task.CompletedTask;
        }
    }
}
