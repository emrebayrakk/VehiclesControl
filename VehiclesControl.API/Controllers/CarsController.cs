using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehiclesControl.Application.Car;
using VehiclesControl.Domain.Entities;
using VehiclesControl.Domain.Input;
using VehiclesControl.Domain.Outs;

namespace VehiclesControl.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        private readonly ICarService _carService;
        private readonly IMapper _mapper;

        #region Constructors

        public CarsController(ICarService carService, IMapper mapper)
        {
            _carService = carService;
            _mapper = mapper;
        }


        #endregion


        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<CarResponse>>), StatusCodes.Status200OK)]
        public ApiResponse<List<CarResponse>> CarList()
        {
            return _carService.CarList();
        }

        [HttpGet("with-dapper")]
        [ProducesResponseType(typeof(ApiResponse<List<CarResponse>>), StatusCodes.Status200OK)]
        public async Task<ApiResponse<List<CarResponse>>> CarListWithDapper()
        {
            var res =  await _carService.CarListWithDapper();
            return res;
        }


        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<CarResponse>), StatusCodes.Status200OK)]
        public ApiResponse<CarResponse> GetCar(long id)
        {
            return _carService.GetCar(id);
        }

        [HttpGet("with-dapper/{id}")]
        [ProducesResponseType(typeof(ApiResponse<CarResponse>), StatusCodes.Status200OK)]
        public async Task<ApiResponse<CarResponse>> GetCarWithDapper(long id)
        {
            return await _carService.GetCarWithDapper(id);
        }


        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<long>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<long>), StatusCodes.Status404NotFound)]
        public ApiResponse<long> Create([FromBody] CarRequest car)
        {
            var response = _carService.CreateCar(car);
            return response;
        }
        [HttpPost("rabbitmq")]
        [ProducesResponseType(typeof(ApiResponse<long>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<long>), StatusCodes.Status404NotFound)]
        public ApiResponse<Car> CreateCar([FromBody] CarRequest car)
        {
            var response = _carService.AddCar(car);
            return response;
        }
        [HttpPost("with-dapper-created")]
        [ProducesResponseType(typeof(ApiResponse<long>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<long>), StatusCodes.Status404NotFound)]
        public async Task<ApiResponse<bool>> CreateWithDapper([FromBody] CarRequest car)
        {
            var response = await _carService.CreateCarWithDapper(car);
            return response;
        }
        [HttpDelete("with-dapper-delete/{id}")]
        [ProducesResponseType(typeof(ApiResponse<long>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<long>), StatusCodes.Status404NotFound)]
        public async Task<ApiResponse<bool>> DeleteWithDapper(long id)
        {
            var response = await _carService.DeleteCarWithDapper(id);
            return response;
        }

        [HttpPut("with-dapper-updated")]
        [ProducesResponseType(typeof(ApiResponse<long>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<long>), StatusCodes.Status404NotFound)]
        public async Task<ApiResponse<bool>> UpdateWithDapper([FromBody] CarRequest car)
        {
            var response = await _carService.UpdateCarWithDapper(car);
            return response;
        }


        [HttpPatch("{id}/HeadlightsOn")]
        [ProducesResponseType(typeof(ApiResponse<long>), StatusCodes.Status200OK)]
        public ApiResponse<long> CarHeadlightsOn(long id)
        {
            return _carService.CarByHeadlightsOn(id);
        }

      
        [HttpPatch("{id}/HeadlightsOff")]
        [ProducesResponseType(typeof(ApiResponse<long>), StatusCodes.Status200OK)]
        public ApiResponse<long> CarHeadlightsOff(long id)
        {
            return _carService.CarByHeadlightsOff(id);
        }
    }
}
