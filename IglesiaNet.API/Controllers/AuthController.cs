using IglesiaNet.Application.Auth;
using IglesiaNet.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IglesiaNet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthAppService _auth;
    public AuthController(AuthAppService auth) => _auth = auth;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await _auth.LoginAsync(request, ct);
        if (result.IsSuccess) return Ok(result.Response);

        return result.ErrorCode switch
        {
            "not_admin"   => StatusCode(403, new { message = "Tu cuenta no tiene permisos de administrador." }),
            "wrong_church"=> StatusCode(403, new { message = "No administras esta iglesia." }),
            _             => Unauthorized(new { message = "Correo o contraseña incorrectos." })
        };
    }

    [HttpPost("users")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request, CancellationToken ct)
    {
        try
        {
            var id = await _auth.CreateUserAsync(request, ct);
            return Ok(new { message = "Usuario creado", userId = id });
        }
        catch (DomainException ex) { return BadRequest(new { message = ex.Message }); }
        catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
    }
}
