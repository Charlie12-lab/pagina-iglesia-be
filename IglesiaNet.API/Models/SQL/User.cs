using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IglesiaNet.API.Models.SQL;

public enum UserRole
{
    SuperAdmin,
    ChurchAdmin
}

public class User
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; } = UserRole.ChurchAdmin;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // FK - null = SuperAdmin sin iglesia asignada
    [ForeignKey(nameof(Church))]
    public int? ChurchId { get; set; }
    public Church? Church { get; set; }
}
