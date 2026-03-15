using System.ComponentModel.DataAnnotations;
using IglesiaNet.Domain.Churches;

namespace IglesiaNet.Application.Churches;

public record ChurchDto(
    int Id, string Name, string? Address, string? City,
    string? Phone, string? Email, string? Description,
    string? LogoUrl, string? WebsiteUrl, bool IsActive
)
{
    public static ChurchDto From(Church c) => new(
        c.Id, c.Name, c.Address, c.City, c.Phone,
        c.Email, c.Description, c.LogoUrl, c.WebsiteUrl, c.IsActive);
}

public record CreateChurchRequest(
    [Required, MaxLength(200)] string Name,
    string? Address, string? City, string? Phone,
    string? Email, string? Description, string? LogoUrl, string? WebsiteUrl
);

public record UpdateChurchRequest(
    string? Name, string? Address, string? City, string? Phone,
    string? Email, string? Description, string? LogoUrl, string? WebsiteUrl, bool? IsActive
);
