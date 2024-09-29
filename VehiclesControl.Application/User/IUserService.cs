using VehiclesControl.Domain.Input;
using VehiclesControl.Domain.Inputs;
using VehiclesControl.Domain.Outs;
using VehiclesControl.Domain.Responses;

namespace VehiclesControl.Application.User
{
    public interface IUserService
    {
        ApiResponse<long> Create(UserRequest userInput);
        ApiResponse<Domain.Entities.User> Update(UserRequest userInput);
        ApiResponse<List<UserResponse>> UserList();
        ApiResponse<UserResponse> GetUser(long id);
        ApiResponse<LoginResponse> Login(UserLoginRequest login);
    }
}
