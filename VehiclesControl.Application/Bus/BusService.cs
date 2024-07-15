using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehiclesControl.Domain.Input;
using VehiclesControl.Domain.Interfaces;
using VehiclesControl.Domain.Outs;

namespace VehiclesControl.Application.Bus
{
    public class BusService : IBusService
    {
        public IBusRepo _busRepo;

        public BusService(IBusRepo busRepo)
        {
            _busRepo = busRepo;
        }

        public ApiResponse<long> CreateBus(BusRequest busInput)
        {
            try
            {
                long id = _busRepo.Add(busInput);
                if (id != -1)
                    return new ApiResponse<long>(true, ResultCode.Instance.Ok, "Success", id);
                return new ApiResponse<long>(false, ResultCode.Instance.Failed, "ErrorOccured", -1);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        public ApiResponse<BusResponse> GetBus(long id)
        {
            try
            {
                var result = _busRepo.FirstOrDefaultAsync(x => x.Id == id);
                return new ApiResponse<BusResponse>(true, ResultCode.Instance.Ok, "Success", result);
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }

        public ApiResponse<List<BusResponse>> BusList()
        {
            try
            {
                var result = _busRepo.GetAll();
                return new ApiResponse<List<BusResponse>>(true, ResultCode.Instance.Ok, "Success", result);
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }

        public ApiResponse<BusResponse> GetBusByColor(string color)
        {
            try
            {
                var result = _busRepo.FirstOrDefaultAsync(a => a.Color == color);
                if (result is not null)
                {
                    return new ApiResponse<BusResponse>(true, ResultCode.Instance.Ok, "Success", result);
                }
                return new ApiResponse<BusResponse>(true, ResultCode.Instance.NotFound, "NotFound", null);
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }
    }
}
