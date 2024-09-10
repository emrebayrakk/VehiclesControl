using VehiclesControl.Domain.Entities;

namespace VehiclesControl.Application.RabbitMq
{
    public interface ICarDriverNotificationPublisherService : IDriverNotification<VehiclesControl.Domain.Entities.Car>
    {
    }
}
