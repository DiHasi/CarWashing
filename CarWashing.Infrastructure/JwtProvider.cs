using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CarWashing.Application.Interfaces.Auth;
using CarWashing.Domain.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CarWashing.Infrastructure;

public class JwtProvider(IOptions<JwtOptions> options) : IJwtProvider
{
    private readonly JwtOptions _options = options.Value;

    public string GenerateToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        };
        claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role.ToString())));
        
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)),
            SecurityAlgorithms.HmacSha256
        );
        var token = new JwtSecurityToken
        (
            signingCredentials: signingCredentials,
            expires: DateTime.Now.AddHours(_options.ExpiresHours),
            claims: claims);
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}