using VehiclesControl.Domain.Input;
using VehiclesControl.Domain.Interfaces.Dapper;
using VehiclesControl.Domain.Interfaces.EntityFramework;
using VehiclesControl.Domain.Outs;

namespace VehiclesControl.Application.Car
{
    public class CarService : ICarService
    {
        private readonly ICarRepo _carRepo;
        private readonly ICarRepositoryDapper _carRepositoryDapper;

        public CarService(ICarRepo carRepo, ICarRepositoryDapper carRepositoryDapper)
        {
            _carRepo = carRepo;
            _carRepositoryDapper = carRepositoryDapper;
        }
        public ApiResponse<long> CreateCar(CarRequest carInput)
        {
            try
            {
                long id = _carRepo.Add(carInput);
                if (id != -1)
                    return new ApiResponse<long>(true, ResultCode.Instance.Ok, "Success", id);
                return new ApiResponse<long>(false, ResultCode.Instance.Failed, "ErrorOccured", -1);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public ApiResponse<CarResponse> GetCar(long id)
        {
            try
            {
                var result = _carRepo.FirstOrDefaultAsync(x => x.Id == id);
                return new ApiResponse<CarResponse>(true, ResultCode.Instance.Ok, "Success", result);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public ApiResponse<List<CarResponse>> CarList()
        {
            try
            {
                var result = _carRepo.GetAll();
                return new ApiResponse<List<CarResponse>>(true, ResultCode.Instance.Ok, "Success", result);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public ApiResponse<CarResponse> GetCarByColor(string color)
        {
            try
            {
                var result = _carRepo.FirstOrDefaultAsync(a => a.Color == color);
                if (result is not null)
                {
                    return new ApiResponse<CarResponse>(true, ResultCode.Instance.Ok, "Success", result);
                }
                return new ApiResponse<CarResponse>(true, ResultCode.Instance.NotFound, "NotFound", null);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        public ApiResponse<long> CarByHeadlightsOn(long id)
        {
            try
            {
                var result = _carRepo.FirstOrDefault(x => x.Id == id);
                if (result is not null)
                {
                    result.HeadlightsOn = true;
                    _carRepo.Update(result);
                    return new ApiResponse<long>(true, ResultCode.Instance.Ok, "Success", id);
                }
                return new ApiResponse<long>(false, ResultCode.Instance.NotFound, "NotFound", -1);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        public ApiResponse<long> CarByHeadlightsOff(long id)
        {
            try
            {
                var result = _carRepo.FirstOrDefault(x => x.Id == id);
                if (result is not null)
                {
                    result.HeadlightsOn = false;
                    _carRepo.Update(result);
                    return new ApiResponse<long>(true, ResultCode.Instance.Ok, "Success", id);
                }
                return new ApiResponse<long>(false, ResultCode.Instance.NotFound, "NotFound", -1);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        public ApiResponse<List<CarResponse>> CarListWithDapper()
        {
            try
            {
                var result = _carRepositoryDapper.GetAll();
                return new ApiResponse<List<CarResponse>>(true, ResultCode.Instance.Ok, "Success", result);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public ApiResponse<CarResponse> GetCarWithDapper(long id)
        {
            try
            {
                var result = _carRepositoryDapper.GetById(id);
                return new ApiResponse<CarResponse>(true, ResultCode.Instance.Ok, "Success", result);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public ApiResponse<long> CreateCarWithDapper(CarRequest carInput)
        {
            try
            {
                long id = _carRepositoryDapper.AddCar(carInput);
                if (id != -1)
                    return new ApiResponse<long>(true, ResultCode.Instance.Ok, "Success", id);
                return new ApiResponse<long>(false, ResultCode.Instance.Failed, "ErrorOccured", -1);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
