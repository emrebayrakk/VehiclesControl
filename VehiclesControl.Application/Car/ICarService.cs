using VehiclesControl.Domain.Input;
using VehiclesControl.Domain.Outs;

namespace VehiclesControl.Application.Car
{
    public interface ICarService
    {
        ApiResponse<long> CreateCar(CarRequest carInput);
        ApiResponse<Domain.Entities.Car> AddCar(CarRequest carInput);
        ApiResponse<List<CarResponse>> CarList();
        ApiResponse<CarResponse> GetCar(long id);
        ApiResponse<CarResponse> GetCarByColor(string color);
        ApiResponse<long> CarByHeadlightsOn(long id);
        ApiResponse<long> CarByHeadlightsOff(long id);
        ApiResponse<List<CarResponse>> CarListWithDapper();
        ApiResponse<CarResponse> GetCarWithDapper(long id);
        ApiResponse<long> CreateCarWithDapper(CarRequest carInput);
        ApiResponse<long> UpdateCarWithDapper(CarRequest carInput);
        ApiResponse<long> DeleteCarWithDapper(long id);
    }
}
