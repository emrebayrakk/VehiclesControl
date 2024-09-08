using VehiclesControl.Domain.Input;
using VehiclesControl.Domain.Outs;

namespace VehiclesControl.Domain.Interfaces.Dapper
{
    public interface ICarRepositoryDapper
    {
        List<CarResponse> GetAll();
        CarResponse GetById(long id);
        long AddCar(UserRequest userRequest);
        long UpdateCar(UserRequest userRequest);
        long DeleteCar(long id);
    }
}
