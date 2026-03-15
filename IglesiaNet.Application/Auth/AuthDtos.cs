using System.ComponentModel.DataAnnotations;

namespace IglesiaNet.Application.Auth;

public record LoginRequest(
    [Required] string Email,
    [Required] string Password
);

public record LoginResponse(
    string Token,
    string Username,
    string Role,
    int? ChurchId,
    string? ChurchName
);

public record CreateUserRequest(
    [Required, MaxLength(100)] string Username,
    [Required] string Email,
    [Required, MinLength(8)] string Password,
    [Required] string Role,
    int? ChurchId
);
