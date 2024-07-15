using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using VehiclesControl.Application.User;
using VehiclesControl.Domain.Input;
using VehiclesControl.Domain.Inputs;
using VehiclesControl.Domain.Outs;

namespace VehiclesControl.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        #region Constructors

        public AuthController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
        }


        #endregion


        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        public ApiResponse<string> Create([FromBody] UserLoginRequest user)
        {
            var response = _userService.Login(user);
            return response;
        }
    }
}
