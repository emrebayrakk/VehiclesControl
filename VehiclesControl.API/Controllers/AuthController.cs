using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using VehiclesControl.Application.User;
using VehiclesControl.Domain.Input;
using VehiclesControl.Domain.Inputs;
using VehiclesControl.Domain.Outs;
using VehiclesControl.Domain.Responses;

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


        [HttpPost("Login")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        public ApiResponse<LoginResponse> Create([FromBody] UserLoginRequest user)
        {
            var response = _userService.Login(user);
            return response;
        }

        [HttpGet("get-ip")]
        public ApiResponse<string> GetClientIp()
        {
            var ipAddress = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();

            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = HttpContext.Connection.RemoteIpAddress?.MapToIPv6().ToString();
                if (ipAddress is null)
                {
                    return new ApiResponse<string>(true, ResultCode.Instance.NotFound, "Success", "");
                }
            }
            return new ApiResponse<string>(true, ResultCode.Instance.Ok, "Success", ipAddress); 
        }
    }
}
