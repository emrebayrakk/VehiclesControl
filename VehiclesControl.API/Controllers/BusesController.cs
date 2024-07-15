using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehiclesControl.Application.Bus;
using VehiclesControl.Application.Car;
using VehiclesControl.Domain.Input;
using VehiclesControl.Domain.Outs;

namespace VehiclesControl.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BusesController : ControllerBase
    {
        private readonly IBusService _busService;
        private readonly IMapper _mapper;

        #region Constructors

        public BusesController(IBusService busService, IMapper mapper)
        {
            _busService = busService;
            _mapper = mapper;
        }


        #endregion

        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<BusResponse>>), StatusCodes.Status200OK)]
        public ApiResponse<List<BusResponse>> BusList()
        {
            return _busService.BusList();
        }

        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<BusResponse>), StatusCodes.Status200OK)]
        public ApiResponse<BusResponse> GetBus(long id)
        {
            return _busService.GetBus(id);
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<long>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<long>), StatusCodes.Status404NotFound)]
        public ApiResponse<long> Create([FromBody] BusRequest bus)
        {
            var response = _busService.CreateBus(bus);
            return response;
        }
    }
}
