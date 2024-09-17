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
        Task<ApiResponse<List<CarResponse>>> CarListWithDapper();
        Task<ApiResponse<bool>> CreateCarWithDapper(CarRequest carInput);
        Task<ApiResponse<bool>> UpdateCarWithDapper(CarRequest carInput);
        Task<ApiResponse<bool>> DeleteCarWithDapper(long id);
        Task<ApiResponse<CarResponse>> GetCarWithDapper(long id);
    }
}
