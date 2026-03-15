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

    public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await _users.GetByEmailAsync(request.Email.ToLowerInvariant(), ct);
        if (user is null || !user.IsActive) return null;
        if (!_hasher.Verify(request.Password, user.PasswordHash)) return null;

        var token = _jwt.Generate(user);
        return new LoginResponse(token, user.Username, user.Role.ToString(), user.ChurchId, null);
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
