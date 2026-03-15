using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IglesiaNet.Application.Common;
using IglesiaNet.Domain.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace IglesiaNet.Infrastructure.Authentication;

public class JwtTokenProvider : IJwtTokenProvider
{
    private readonly IConfiguration _config;
    public JwtTokenProvider(IConfiguration config) => _config = config;

    public string Generate(User user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Secret"]
                ?? throw new InvalidOperationException("JWT Secret not configured")));

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("churchId", user.ChurchId?.ToString() ?? "")
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
