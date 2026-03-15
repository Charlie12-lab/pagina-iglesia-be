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
        return result is null
            ? Unauthorized(new { message = "Credenciales incorrectas" })
            : Ok(result);
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
