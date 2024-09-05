using VehiclesControl.Domain.Outs;

namespace VehiclesControl.Domain.Interfaces.Dapper
{
    public interface ICarRepositoryDapper
    {
        List<CarResponse> GetAll();
    }
}
