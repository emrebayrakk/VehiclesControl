using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehiclesControl.Application.Car;
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

    
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<CarResponse>), StatusCodes.Status200OK)]
        public ApiResponse<CarResponse> GetCar(long id)
        {
            return _carService.GetCar(id);
        }

    
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<long>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<long>), StatusCodes.Status404NotFound)]
        public ApiResponse<long> Create([FromBody] CarRequest car)
        {
            var response = _carService.CreateCar(car);
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
