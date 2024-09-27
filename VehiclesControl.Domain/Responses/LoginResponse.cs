using VehiclesControl.Domain.Outs;

namespace VehiclesControl.Domain.Responses
{
    public class LoginResponse
    {
        public UserResponse User { get; set; }
        public string Token { get; set; }
    }
}
