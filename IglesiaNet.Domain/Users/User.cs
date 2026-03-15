using IglesiaNet.Domain.Shared;

namespace IglesiaNet.Domain.Users;

public enum UserRole { SuperAdmin, ChurchAdmin }

public class User : Entity<int>
{
    public string Username { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public int? ChurchId { get; private set; }

    // Para EF Core
    private User() : base()
    {
        Username = string.Empty; Email = string.Empty; PasswordHash = string.Empty;
    }

    private User(string username, string email, string passwordHash, UserRole role, int? churchId)
        : base()
    {
        Username = username; Email = email; PasswordHash = passwordHash;
        Role = role; ChurchId = churchId;
        IsActive = true; CreatedAt = DateTime.UtcNow;
    }

    public static User Create(string username, string email, string passwordHash, UserRole role, int? churchId)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new DomainException("El nombre de usuario es requerido");
        if (role == UserRole.ChurchAdmin && !churchId.HasValue)
            throw new DomainException("Un ChurchAdmin debe tener una iglesia asignada");

        Shared.Email.Create(email); // Valida formato
        return new User(username.Trim(), email.Trim().ToLowerInvariant(), passwordHash, role, churchId);
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
