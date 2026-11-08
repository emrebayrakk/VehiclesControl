using VehiclesControl.Domain.Entities;

namespace VehiclesControl.Application.Abstractions;

public interface IJwtProvider
{
    (string Token, DateTime ExpiresAt) CreateToken(User user);
}
