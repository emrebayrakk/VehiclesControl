using VehiclesControl.Application.RabbitMq;
using VehiclesControl.Domain.Entities;
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
        private readonly ICarDriverNotificationPublisherService _publisherService;

        public CarService(ICarRepo carRepo, ICarRepositoryDapper carRepositoryDapper, ICarDriverNotificationPublisherService publisherService)
        {
            _carRepo = carRepo;
            _carRepositoryDapper = carRepositoryDapper;
            _publisherService = publisherService;
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

        public async Task<ApiResponse<List<CarResponse>>> CarListWithDapper()
        {
            try
            {
                var result = await _carRepositoryDapper.GetAllAsync();
                var carResponseList = result.Select(car => new CarResponse
                {
                    Id = car.Id,
                    Color = car.Color,
                    HeadlightsOn = car.HeadlightsOn,
                    Wheels = car.Wheels,
                    CreatedDate = car.CreatedDate,
                }).ToList();
                return new ApiResponse<List<CarResponse>>(true, ResultCode.Instance.Ok, "Success", carResponseList);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<ApiResponse<CarResponse>> GetCarWithDapper(long id)
        {
            try
            {
                var result = await _carRepositoryDapper.GetByIdAsync(id);
                var mappedCar = new CarResponse
                {
                    Color = result.Color,
                    HeadlightsOn = result.HeadlightsOn,
                    Wheels = result.Wheels,
                    Id = result.Id,
                    CreatedDate = result.CreatedDate
                };
                return new ApiResponse<CarResponse>(true, ResultCode.Instance.Ok, "Success", mappedCar);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<ApiResponse<bool>> CreateCarWithDapper(CarRequest carInput)
        {
            try
            {

                var newCar = new Domain.Entities.Car
                {
                    Color = carInput.Color,
                    CreatedDate = DateTime.Now,
                    HeadlightsOn = carInput.HeadlightsOn,
                    Wheels = carInput.Wheels,
                };
                var added = await _carRepositoryDapper.InsertAsync(newCar);
                if (added)
                    return new ApiResponse<bool>(true, ResultCode.Instance.Ok, "Success", added);
                return new ApiResponse<bool>(false, ResultCode.Instance.Failed, "ErrorOccured", false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ApiResponse<bool>> UpdateCarWithDapper(CarRequest carInput)
        {
            try
            {
                var car = await GetCarWithDapper(carInput.Id.GetValueOrDefault());
                if (car.Data is not null)
                {
                    var newCar = new Domain.Entities.Car
                    {
                        UpdatedDate = DateTime.Now,
                        Color = carInput.Color,
                        HeadlightsOn = carInput.HeadlightsOn,
                        Id = carInput.Id.GetValueOrDefault(),
                        Wheels = carInput.Wheels,
                        CreatedDate = car.Data.CreatedDate,
                    };
                    var updated = await _carRepositoryDapper.UpdateAsync(newCar);
                    if (updated)
                        return new ApiResponse<bool>(true, ResultCode.Instance.Ok, "Success", updated);
                    return new ApiResponse<bool>(false, ResultCode.Instance.Failed, "ErrorOccured", false);
                }
                else
                {
                    return new ApiResponse<bool>(false, ResultCode.Instance.NotFound, "NotFound", false);
                }

                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ApiResponse<bool>> DeleteCarWithDapper(long id)
        {
            try
            {
                var res = await _carRepositoryDapper.DeleteAsync(id);
                if (res)
                    return new ApiResponse<bool>(true, ResultCode.Instance.Ok, "Success", res);
                return new ApiResponse<bool>(false, ResultCode.Instance.Failed, "ErrorOccured", false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ApiResponse<Domain.Entities.Car> AddCar(CarRequest carInput)
        {
            try
            {
                var res = _carRepo.AddEntity(carInput);
                _publisherService.SendNotification(res.Id, res);
                if (res != null)
                    return new ApiResponse<Domain.Entities.Car>(true, ResultCode.Instance.Ok, "Success", res);
                return new ApiResponse<Domain.Entities.Car>(false, ResultCode.Instance.Failed, "ErrorOccured", null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
