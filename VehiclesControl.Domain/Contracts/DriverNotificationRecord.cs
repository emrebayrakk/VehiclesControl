namespace VehiclesControl.Domain.Contracts
{
    public record DriverNotificationRecord<T>(long driverId, T data);
}
