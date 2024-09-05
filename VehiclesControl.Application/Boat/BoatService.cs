using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehiclesControl.Domain.Input;
using VehiclesControl.Domain.Interfaces.EntityFramework;
using VehiclesControl.Domain.Outs;

namespace VehiclesControl.Application.Boat
{
    public class BoatService : IBoatService
    {
        public IBoatRepo _boatRepo;

        public BoatService(IBoatRepo boatRepo)
        {
            _boatRepo = boatRepo;
        }
        public ApiResponse<long> CreateBoat(BoatRequest boatInput)
        {
            try
            {
                long id = _boatRepo.Add(boatInput);
                if (id != -1)
                    return new ApiResponse<long>(true, ResultCode.Instance.Ok, "Success", id);
                return new ApiResponse<long>(false, ResultCode.Instance.Failed, "ErrorOccured", -1);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        public ApiResponse<BoatResponse> GetBoat(long id)
        {
            try
            {
                var result = _boatRepo.FirstOrDefaultAsync(x => x.Id == id);
                return new ApiResponse<BoatResponse>(true, ResultCode.Instance.Ok, "Success", result);
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }

        public ApiResponse<List<BoatResponse>> BoatList()
        {
            try
            {
                var result = _boatRepo.GetAll();
                return new ApiResponse<List<BoatResponse>>(true, ResultCode.Instance.Ok, "Success", result);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        public ApiResponse<BoatResponse> GetBoatByColor(string color)
        {
            try
            {
                var result = _boatRepo.FirstOrDefaultAsync(a => a.Color == color);
                if (result is not null)
                {
                    return new ApiResponse<BoatResponse>(true, ResultCode.Instance.Ok, "Success", result);
                }
                return new ApiResponse<BoatResponse>(true, ResultCode.Instance.NotFound, "NotFound", null);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }
    }
}
