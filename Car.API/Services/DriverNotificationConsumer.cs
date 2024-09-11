using Car.API.Data.Entities;
using MassTransit;
using VehiclesControl.Domain.Contracts;
using VehiclesControl.Domain.Entities;

namespace Car.API.Services
{
    public class DriverNotificationConsumer : IConsumer<DriverNotificationRecord<VehiclesControl.Domain.Entities.Car>>
    {
        private readonly ILogger<DriverNotificationConsumer> _logger;
        private readonly ICarService _carService;

        public DriverNotificationConsumer(ILogger<DriverNotificationConsumer> logger, ICarService carService)
        {
            _logger = logger;
            _carService = carService;
        }

        public Task Consume(ConsumeContext<DriverNotificationRecord<VehiclesControl.Domain.Entities.Car>> context)
        {
            _logger.LogInformation("Id: " + context.Message.driverId + " Car color: " + context.Message.data.Color
                + " Created Date: " + context.Message.data.CreatedDate.ToString("d"));
            var car = new Car.API.Data.Entities.Car
            {
                CarId = context.Message.data.Id,
                Color = context.Message.data.Color,
                CreatedDate = context.Message.data.CreatedDate,
                HeadlightsOn = context.Message.data.HeadlightsOn,
                Wheels = context.Message.data.Wheels,
            };
            _carService.AddCarAsync(car);
            return Task.CompletedTask;
        }
    }
}
