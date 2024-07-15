using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehiclesControl.Application.Boat;
using VehiclesControl.Application.Bus;
using VehiclesControl.Domain.Input;
using VehiclesControl.Domain.Outs;

namespace VehiclesControl.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BoatsController : ControllerBase
    {
        private readonly IBoatService _boatService;
        private readonly IMapper _mapper;

        #region Constructors

        public BoatsController(IBoatService boatService, IMapper mapper)
        {
            _boatService = boatService;
            _mapper = mapper;
        }


        #endregion

        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<BoatResponse>>), StatusCodes.Status200OK)]
        public ApiResponse<List<BoatResponse>> BusList()
        {
            return _boatService.BoatList();
        }

        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<BoatResponse>), StatusCodes.Status200OK)]
        public ApiResponse<BoatResponse> GetBoat(long id)
        {
            return _boatService.GetBoat(id);
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<long>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<long>), StatusCodes.Status404NotFound)]
        public ApiResponse<long> Create([FromBody] BoatRequest boat)
        {
            var response = _boatService.CreateBoat(boat);
            return response;
        }
    }
}
