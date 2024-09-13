using Car.API.Data.Entities;
using VehiclesControl.Domain.Entities;

namespace Car.API.Services
{
    public interface ICarService
    {
        Task AddCarAsync(Car.API.Data.Entities.Car car);
        Task<List<Car.API.Data.Entities.Car>> GetCars();
    }
}
