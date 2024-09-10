

using MassTransit;
using VehiclesControl.Domain.Contracts;
using VehiclesControl.Domain.Entities;

namespace VehiclesControl.Application.RabbitMq
{
    public class CarDriverNotificationPublisherService : ICarDriverNotificationPublisherService
    {
        private readonly IPublishEndpoint _publish;

        public CarDriverNotificationPublisherService(IPublishEndpoint publish)
        {
            _publish = publish;
        }

        public async Task SendNotification(long driverId, Domain.Entities.Car data)
        {
            await _publish.Publish(new DriverNotificationRecord<Domain.Entities.Car>(driverId, data));
        }
    }
}
