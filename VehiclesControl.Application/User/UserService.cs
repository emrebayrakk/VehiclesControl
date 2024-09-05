using VehiclesControl.Domain.Input;
using VehiclesControl.Domain.Inputs;
using VehiclesControl.Domain.Interfaces.EntityFramework;
using VehiclesControl.Domain.Outs;

namespace VehiclesControl.Application.User
{
    public class UserService : IUserService
    {
        public IUserRepo _userRepo;
        private readonly IJwtProvider _jwtProvider;

        public UserService(IUserRepo userRepo, IJwtProvider jwtProvider)
        {
            _userRepo = userRepo;
            _jwtProvider = jwtProvider;
        }

        public ApiResponse<long> Create(UserRequest userInput)
        {
            long id = _userRepo.Add(userInput);
            if (id != -1)
                return new ApiResponse<long>(true, ResultCode.Instance.Ok, "Success", id);
            return new ApiResponse<long>(false, ResultCode.Instance.Failed, "ErrorOccured", -1);
        }

        public ApiResponse<UserResponse> GetUser(long id)
        {
            var result = _userRepo.FirstOrDefaultAsync(x => x.Id == id);
            return new ApiResponse<UserResponse>(true, ResultCode.Instance.Ok,"Success", result);
        }

        public ApiResponse<string> Login(UserLoginRequest login)
        {
            var result = _userRepo.FirstOrDefault(a => a.Email == login.Email && a.Password == login.Password);
            if (result is not null)
            {
                var token = _jwtProvider.CreateToken(result);
                return new ApiResponse<string>(true, ResultCode.Instance.Ok, "Success", token);
            }  
            return new ApiResponse<string>(false, ResultCode.Instance.LoginInvalid, "LoginInvalid", "");

        }

        public ApiResponse<List<UserResponse>> UserList()
        {
            var result = _userRepo.GetAll();
            return new ApiResponse<List<UserResponse>>(true, ResultCode.Instance.Ok, "Success", result);
        }
    }
}
