using Car.API.Data.Entities;

namespace Car.API.Services
{
    public interface ICarService
    {
        public Task AddCarAsync(Car.API.Data.Entities.Car car);
    }
}
