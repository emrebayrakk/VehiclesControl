using VehiclesControl.Domain.Input;
using VehiclesControl.Domain.Outs;

namespace VehiclesControl.Application.Bus
{
    public interface IBusService
    {
        ApiResponse<long> CreateBus(BusRequest busInput);
        ApiResponse<List<BusResponse>> BusList();
        ApiResponse<BusResponse> GetBus(long id);
        ApiResponse<BusResponse> GetBusByColor(string color);
    }
}
