using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehiclesControl.Domain.Input;
using VehiclesControl.Domain.Outs;

namespace VehiclesControl.Application.Car
{
    public interface ICarService
    {
        ApiResponse<long> CreateCar(CarRequest carInput);
        ApiResponse<List<CarResponse>> CarList();
        ApiResponse<CarResponse> GetCar(long id);
        ApiResponse<CarResponse> GetCarByColor(string color);
        ApiResponse<long> CarByHeadlightsOn(long id);
        ApiResponse<long> CarByHeadlightsOff(long id);
    }
}
