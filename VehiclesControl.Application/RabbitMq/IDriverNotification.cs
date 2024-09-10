namespace VehiclesControl.Application.RabbitMq
{
    public interface IDriverNotification<T>
    {
        Task SendNotification(long driverId, T data);
    }
}
