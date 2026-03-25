using System.ComponentModel.DataAnnotations;

namespace IglesiaNet.Application.Auth;

public record LoginRequest(
    [Required] string Email,
    [Required] string Password,
    bool ExpectAdmin = false,
    int? ChurchId = null
);

public record LoginResponse(
    string Token,
    string Username,
    string Email,
    string Role,
    int? ChurchId,
    string? ChurchName
);

public record LoginResult(LoginResponse? Response = null, string? ErrorCode = null)
{
    public bool IsSuccess => Response is not null;
    public static LoginResult Ok(LoginResponse r) => new(Response: r);
    public static LoginResult Fail(string code) => new(ErrorCode: code);
};

public record CreateUserRequest(
    [Required, MaxLength(100)] string Username,
    [Required] string Email,
    [Required, MinLength(8)] string Password,
    [Required] string Role,
    int? ChurchId
);
