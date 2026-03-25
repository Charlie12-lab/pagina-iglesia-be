using IglesiaNet.Application.Common;
using IglesiaNet.Domain.Users;

namespace IglesiaNet.Application.Auth;

public class AuthAppService
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenProvider _jwt;

    public AuthAppService(IUserRepository users, IPasswordHasher hasher, IJwtTokenProvider jwt)
    {
        _users = users;
        _hasher = hasher;
        _jwt = jwt;
    }

    public async Task<LoginResult> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await _users.GetByEmailAsync(request.Email.ToLowerInvariant(), ct);
        if (user is null || !user.IsActive) return LoginResult.Fail("invalid_credentials");
        if (!_hasher.Verify(request.Password, user.PasswordHash)) return LoginResult.Fail("invalid_credentials");

        // Validar que tenga rol admin si se requiere
        if (request.ExpectAdmin &&
            user.Role != UserRole.SuperAdmin &&
            user.Role != UserRole.ChurchAdmin)
            return LoginResult.Fail("not_admin");

        // ChurchAdmin: validar que administra la iglesia seleccionada
        if (request.ExpectAdmin && request.ChurchId.HasValue && user.Role == UserRole.ChurchAdmin)
        {
            if (user.ChurchId != request.ChurchId)
                return LoginResult.Fail("wrong_church");
        }

        var token = _jwt.Generate(user);
        var response = new LoginResponse(token, user.Username, user.Email, user.Role.ToString(), user.ChurchId, null);
        return LoginResult.Ok(response);
    }

    public async Task<int> CreateUserAsync(CreateUserRequest request, CancellationToken ct = default)
    {
        if (!Enum.TryParse<UserRole>(request.Role, out var role))
            throw new ArgumentException($"Rol inválido: '{request.Role}'. Use SuperAdmin o ChurchAdmin");

        var hash = _hasher.Hash(request.Password);
        var user = User.Create(request.Username, request.Email, hash, role, request.ChurchId);

        await _users.AddAsync(user, ct);
        await _users.SaveChangesAsync(ct);
        return user.Id;
    }
}
