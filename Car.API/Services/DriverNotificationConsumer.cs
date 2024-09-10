using MassTransit;
using VehiclesControl.Domain.Contracts;
using VehiclesControl.Domain.Entities;

namespace Car.API.Services
{
    public class DriverNotificationConsumer : IConsumer<DriverNotificationRecord<VehiclesControl.Domain.Entities.Car>>
    {
        private readonly ILogger<DriverNotificationConsumer> _logger;

        public DriverNotificationConsumer(ILogger<DriverNotificationConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<DriverNotificationRecord<VehiclesControl.Domain.Entities.Car>> context)
        {
            _logger.LogInformation("Id: " + context.Message.driverId + " Car color: " + context.Message.data.Color
                + " Created Date: " + context.Message.data.CreatedDate.ToString("d"));
            return Task.CompletedTask;
        }
    }
}
