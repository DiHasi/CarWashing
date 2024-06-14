using CarWashing.Domain.Models;

namespace CarWashing.Application.Interfaces.Auth;

public interface IJwtProvider
{
    string GenerateToken(User user);
}