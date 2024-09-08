using VehiclesControl.Domain.Input;
using VehiclesControl.Domain.Outs;

namespace VehiclesControl.Domain.Interfaces.Dapper
{
    public interface ICarRepositoryDapper
    {
        List<CarResponse> GetAll();
        CarResponse GetById(long id);
        long AddCar(CarRequest carRequest);
        long UpdateCar(CarRequest carRequest);
        long DeleteCar(long id);
    }
}
