using VehiclesControl.Domain.Input;
using VehiclesControl.Domain.Outs;

namespace VehiclesControl.Application.Boat
{
    public interface IBoatService
    {
        ApiResponse<long> CreateBoat(BoatRequest boatInput);
        ApiResponse<List<BoatResponse>> BoatList();
        ApiResponse<BoatResponse> GetBoat(long id);
        ApiResponse<BoatResponse> GetBoatByColor(string color);
    }
}
